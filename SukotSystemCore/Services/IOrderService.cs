using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    public interface IOrderService
    {
        // Customer-facing (ownership is always enforced against the caller's own JWT id, never
        // trusted from the route/body - a request for someone else's order looks like a 404,
        // not a 403, so its existence is never leaked).
        Task<OrderResponseDTO> CreateOrder(int? customerId, OrderCreateDTO order, CancellationToken cancellationToken);
        Task<OrderResponseDTO> UpdateMyOrder(int customerId, int orderId, OrderCreateDTO order, CancellationToken cancellationToken);
        Task<OrderResponseDTO> GetMyOrderById(int customerId, int orderId, CancellationToken cancellationToken);
        Task<PagedResultDTO<OrderResponseDTO>> GetMyOrders(int customerId, int page, int pageSize, CancellationToken cancellationToken);

        // Rabbi-facing.
        Task<PagedResultDTO<OrderRabbiDTO>> GetAvailableOrders(int? cityId, int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResultDTO<OrderRabbiDTO>> GetMyHandledOrders(int rabbiId, int page, int pageSize, CancellationToken cancellationToken);

        // THE Part C action - optimistic-concurrency protected in the Repository/DataContex.
        Task<OrderRabbiDTO> ClaimOrder(int rabbiId, int orderId, CancellationToken cancellationToken);

        // Only the rabbi who claimed this exact order may complete it (403 otherwise).
        Task<OrderRabbiDTO> CompleteOrder(int rabbiId, int orderId, OrderCompleteDTO complete, CancellationToken cancellationToken);

        // Admin-facing - system-wide, optionally filtered by status.
        Task<PagedResultDTO<OrderAdminDTO>> GetAllOrders(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken);
        Task<OrderAdminDTO> GetOrderAdminViewById(int orderId, CancellationToken cancellationToken);
    }
}
