using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.Enums;
using SukotSystemCore.Exceptions;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderResponseDTO> CreateOrder(int? customerId, OrderCreateDTO order, CancellationToken cancellationToken)
        {
            var orderMap = _mapper.Map<Order>(order);

            if (customerId != null)
            {

                orderMap.CustomerId = customerId;
            }
            else
            {
                orderMap.CallerFullName = order.CallerFullName;
                orderMap.CallerPhone = order.CallerPhone;
            }
            orderMap.Status = OrderStatus.New;
            orderMap.RequestedDate = DateTime.UtcNow;

            var res = await _orderRepository.AddOrder(orderMap, cancellationToken);

            return _mapper.Map<OrderResponseDTO>(res);
        }

        public async Task<OrderResponseDTO> UpdateMyOrder(int customerId, int orderId, OrderCreateDTO order, CancellationToken cancellationToken)
        {
            var existing = await GetOwnedTrackedOrder(customerId, orderId, cancellationToken);

            // Business validation (requirement 2 - belongs in the Service, not the Controller):
            // once a rabbi has claimed it, the request line is no longer the customer's to edit.
            if (existing.Status != OrderStatus.New)
                throw new ConflictException("This request can no longer be edited - it has already been claimed.");

            // In-place map: only CityId/Adress/NeedsTool (the fields OrderCreateDTO actually
            // carries) are touched on the tracked entity - Id/Status/CustomerId/Version and
            // everything else stay exactly as loaded.
            _mapper.Map(order, existing);

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<OrderResponseDTO>(existing);
        }

        public async Task<OrderResponseDTO> GetMyOrderById(int customerId, int orderId, CancellationToken cancellationToken)
        {
            var order = await GetOwnedOrder(customerId, orderId, cancellationToken);
            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task<PagedResultDTO<OrderResponseDTO>> GetMyOrders(int customerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = ClampPaging(page, pageSize);

            var (items, totalCount) = await _orderRepository.GetCustomerOrdersPaged(customerId, page, pageSize, cancellationToken);

            return new PagedResultDTO<OrderResponseDTO>
            {
                Items = _mapper.Map<IEnumerable<OrderResponseDTO>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<OrderRabbiDTO>> GetAvailableOrders(int? cityId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = ClampPaging(page, pageSize);

            var (items, totalCount) = await _orderRepository.GetAvailableOrdersPaged(cityId, page, pageSize, cancellationToken);

            return new PagedResultDTO<OrderRabbiDTO>
            {
                Items = _mapper.Map<IEnumerable<OrderRabbiDTO>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResultDTO<OrderRabbiDTO>> GetMyHandledOrders(int rabbiId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = ClampPaging(page, pageSize);

            var (items, totalCount) = await _orderRepository.GetRabbiHandledOrdersPaged(rabbiId, page, pageSize, cancellationToken);

            return new PagedResultDTO<OrderRabbiDTO>
            {
                Items = _mapper.Map<IEnumerable<OrderRabbiDTO>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderRabbiDTO> ClaimOrder(int rabbiId, int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetTrackedOrderById(orderId, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with id {orderId} was not found.");

            // The business check and the save that follows both act on this SAME tracked
            // instance, loaded once, right above - per the project guide's own warning, a check
            // performed against a separate/earlier read would be worthless. The check alone
            // does NOT prevent the race (two concurrent requests can both pass it against their
            // own stale-but-still-New read) - what actually prevents a double-claim is the
            // Version concurrency token compared at SaveChangesAsync below.
            if (order.Status != OrderStatus.New)
                throw new ConflictException("This request is no longer available - it was already claimed or is not in a claimable state.");

            order.Status = OrderStatus.InProgress;
            order.PerformedRabbiId = rabbiId;
            order.PerformedDate = DateTime.UtcNow;

            try
            {
                await _orderRepository.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                // ExceptionHandlingMiddleware already turns this into 409 + a generic Warning
                // log line - this catch exists only to attach the structured detail (which
                // order, which rabbi lost the race) that the middleware has no way to know,
                // per Part D. The exception itself is rethrown unchanged.
                _logger.LogWarning(
                    "Rabbi {RabbiId} lost the claim race for order {OrderId} - it was already claimed by someone else.",
                    rabbiId, orderId);
                throw;
            }

            return _mapper.Map<OrderRabbiDTO>(order);
        }

        public async Task<OrderRabbiDTO> CompleteOrder(int rabbiId, int orderId, OrderCompleteDTO complete, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetTrackedOrderById(orderId, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with id {orderId} was not found.");

            // 403, not 401/404: this rabbi IS authenticated and IS a legitimate rabbi - they are
            // simply not the one who claimed THIS order. Not found would hide a real order that
            // does exist; 401 would incorrectly say their credentials are the problem.
            if (order.PerformedRabbiId != rabbiId)
                throw new ForbiddenException("Only the rabbi who claimed this request can mark it complete.");

            if (order.Status != OrderStatus.InProgress)
                throw new ConflictException("This request cannot be marked complete from its current status.");

            order.Status = OrderStatus.Completed;
            if (!string.IsNullOrWhiteSpace(complete.Commits))
                order.Commits = complete.Commits;

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<OrderRabbiDTO>(order);
        }

        public async Task<PagedResultDTO<OrderAdminDTO>> GetAllOrders(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken)
        {
            (page, pageSize) = ClampPaging(page, pageSize);

            var (items, totalCount) = await _orderRepository.GetAllOrdersPaged(page, pageSize, status, cancellationToken);

            return new PagedResultDTO<OrderAdminDTO>
            {
                Items = _mapper.Map<IEnumerable<OrderAdminDTO>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderAdminDTO> GetOrderAdminViewById(int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderById(orderId, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with id {orderId} was not found.");

            return _mapper.Map<OrderAdminDTO>(order);
        }

        // Not found AND not-owned are deliberately the same outcome (404) - a customer
        // requesting another customer's order id should never be able to tell the difference
        // between "doesn't exist" and "exists but isn't yours".
        private async Task<Order> GetOwnedOrder(int customerId, int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderById(orderId, cancellationToken);
            if (order == null || order.CustomerId != customerId)
                throw new KeyNotFoundException($"Order with id {orderId} was not found.");
            return order;
        }

        private async Task<Order> GetOwnedTrackedOrder(int customerId, int orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetTrackedOrderById(orderId, cancellationToken);
            if (order == null || order.CustomerId != customerId)
                throw new KeyNotFoundException($"Order with id {orderId} was not found.");
            return order;
        }

        private static (int Page, int PageSize) ClampPaging(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;
            return (page, pageSize);
        }
    }
}
