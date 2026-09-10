using Microsoft.EntityFrameworkCore;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemData
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContex _dataContext;

        public AdminRepository(DataContex dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Admin?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken)
        {
            return await _dataContext.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Phone == phone, cancellationToken);
        }

        public async Task<Admin?> GetAdminById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<Admin> UpdateAdmin(int id, Admin admin, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Admins.FindAsync(new object[] { id }, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"Admin with id {id} was not found.");

            // Same pattern as CustometRepository.UpdateCustomer: the incoming "admin" (mapped
            // straight from AdminUpdateDTO) carries neither Id nor PasswordHash, so both arrive
            // as default(0)/null. Capture the real values before SetValues overwrites them.
            var realId = existing.Id;
            var realPasswordHash = existing.PasswordHash;

            _dataContext.Entry(existing).CurrentValues.SetValues(admin);

            existing.Id = realId;
            existing.PasswordHash = realPasswordHash;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }
    }
}
