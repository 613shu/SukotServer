using SukotSystemCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Repositories
{
    // Deliberately minimal, per the user's explicit choice: Admin is a self-service-only entity
    // in this project (3 fixed/seeded accounts, no self-registration, no roster listing, no
    // admin-creates-admin). This interface only ever needs to resolve a login and let an Admin
    // read/update their own row.
    public interface IAdminRepository
    {
        Task<Admin?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken);
        Task<Admin?> GetAdminById(int id, CancellationToken cancellationToken);
        Task<Admin> UpdateAdmin(int id, Admin admin, CancellationToken cancellationToken);
    }
}
