using Microsoft.EntityFrameworkCore;
using SukotSystemCore.Enums;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemData
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContex _dataContext;

        public OrderRepository(DataContex dataContext)
        {
            _dataContext = dataContext;
        }

        // Every read query below includes City + RequestedCustomer + PerformedRabbi uniformly.
        // Different response DTOs (Customer/Rabbi/Admin views) need different subsets of these,
        // but loading all three consistently keeps every query here N+1-free (requirement 7)
        // without needing a separate Include list per caller.
        private IQueryable<Order> WithIncludes(IQueryable<Order> query) =>
            query.Include(o => o.City).Include(o => o.RequestedCustomer).Include(o => o.PerformedRabbi);

        public async Task<Order> AddOrder(Order order, CancellationToken cancellationToken)
        {
            await _dataContext.Orders.AddAsync(order, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            // Only City needs an explicit reload for mapping (CityName) - RequestedCustomer/
            // PerformedRabbi aren't needed on the just-created Order's response DTO, and
            // PerformedRabbi is null at creation anyway.
            await _dataContext.Entry(order).Reference(o => o.City).LoadAsync(cancellationToken);

            return order;
        }

        public async Task<Order?> GetTrackedOrderById(int id, CancellationToken cancellationToken)
        {
            return await WithIncludes(_dataContext.Orders)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<Order?> GetOrderById(int id, CancellationToken cancellationToken)
        {
            return await WithIncludes(_dataContext.Orders.AsNoTracking())
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetCustomerOrdersPaged(int customerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = WithIncludes(_dataContext.Orders.AsNoTracking())
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.RequestedDate);

            return await PageAsync(query, page, pageSize, cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAvailableOrdersPaged(int? cityId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = WithIncludes(_dataContext.Orders.AsNoTracking())
                .Where(o => o.Status == OrderStatus.New);

            if (cityId.HasValue)
                query = query.Where(o => o.CityId == cityId.Value);

            query = query.OrderBy(o => o.RequestedDate);

            return await PageAsync(query, page, pageSize, cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetRabbiHandledOrdersPaged(int rabbiId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = WithIncludes(_dataContext.Orders.AsNoTracking())
                .Where(o => o.PerformedRabbiId == rabbiId)
                .OrderByDescending(o => o.RequestedDate);

            return await PageAsync(query, page, pageSize, cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllOrdersPaged(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken)
        {
            var query = WithIncludes(_dataContext.Orders.AsNoTracking());

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            query = query.OrderByDescending(o => o.RequestedDate);

            return await PageAsync(query, page, pageSize, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
            => _dataContext.SaveChangesAsync(cancellationToken);

        private static async Task<(IEnumerable<Order> Items, int TotalCount)> PageAsync(
            IQueryable<Order> query, int page, int pageSize, CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
