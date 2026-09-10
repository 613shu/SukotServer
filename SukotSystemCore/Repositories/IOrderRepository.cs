using SukotSystemCore.Enums;
using SukotSystemCore.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddOrder(Order order, CancellationToken cancellationToken);

        // Tracked (NOT AsNoTracking) - the caller mutates the returned entity's fields directly
        // and then calls SaveChangesAsync below. This is deliberate: Part C's optimistic-
        // concurrency check depends on EF comparing the Version value AS IT WAS WHEN LOADED HERE
        // against the database at save time. Used by claim, complete, and a customer's own
        // pending-order edit - anything that mutates an existing Order.
        Task<Order?> GetTrackedOrderById(int id, CancellationToken cancellationToken);

        // AsNoTracking, full includes - for read-only single-order views (Customer/Rabbi/Admin).
        Task<Order?> GetOrderById(int id, CancellationToken cancellationToken);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetCustomerOrdersPaged(int customerId, int page, int pageSize, CancellationToken cancellationToken);

        // status is always New here by construction (the Service passes it), but kept as a
        // parameter rather than hard-coded so the query stays reusable/testable.
        Task<(IEnumerable<Order> Items, int TotalCount)> GetAvailableOrdersPaged(int? cityId, int page, int pageSize, CancellationToken cancellationToken);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetRabbiHandledOrdersPaged(int rabbiId, int page, int pageSize, CancellationToken cancellationToken);

        Task<(IEnumerable<Order> Items, int TotalCount)> GetAllOrdersPaged(int page, int pageSize, OrderStatus? status, CancellationToken cancellationToken);

        // Persists whatever mutation was made on an entity obtained from GetTrackedOrderById.
        // Throws DbUpdateConcurrencyException (-> 409 via ExceptionHandlingMiddleware) if
        // Version no longer matches what was loaded - this is Part C's actual enforcement point.
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
