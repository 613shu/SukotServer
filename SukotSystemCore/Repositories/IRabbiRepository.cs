using SukotSystemCore.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Repositories
{
    public interface IRabbiRepository
    {
        Task<Rabbi> AddRabbi(Rabbi rabbi, CancellationToken cancellationToken);

  
        Task<Rabbi> UpdateRabbi(int id, Rabbi rabbi, IEnumerable<int> coveredCityIds, CancellationToken cancellationToken);

    
        Task<Rabbi> DeactivateRabbi(int id, CancellationToken cancellationToken);
        Task<Rabbi> ReactivateRabbi(int id, CancellationToken cancellationToken);

        Task<Rabbi?> GetRabbiById(int id, CancellationToken cancellationToken);

        // Same as GetRabbiById but eager-loads HandledOrders (+ each order's City/Customer)
        // for the Admin detail view - avoids an N+1 when RabbiAdminDTO.Orders is mapped.
        Task<Rabbi?> GetRabbiByIdWithOrders(int id, CancellationToken cancellationToken);

        // Real database pagination (Skip/Take in the query) - requirement 6.
        Task<(IEnumerable<Rabbi> Items, int TotalCount)> GetAllRabbisPaged(int page, int pageSize, CancellationToken cancellationToken);

        // Used for phone+password login. Matches either registered phone number (Phone or Phone2).
        Task<Rabbi?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken);

        Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken);
    }
}
