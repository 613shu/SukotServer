using Microsoft.EntityFrameworkCore;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemData
{
    public class RabbiRepository : IRabbiRepository
    {
        private readonly DataContex _dataContext;

        public RabbiRepository(DataContex dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Rabbi> AddRabbi(Rabbi rabbi, CancellationToken cancellationToken)
        {
            await _dataContext.Rabbis.AddAsync(rabbi, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            // Reload with HomeCity attached so the caller can map HomeCityName without a
            // second round trip - the entity as passed in only carries HomeCityId.
            await _dataContext.Entry(rabbi).Reference(r => r.HomeCity).LoadAsync(cancellationToken);

            return rabbi;
        }

        public async Task<Rabbi> UpdateRabbi(int id, Rabbi rabbi, IEnumerable<int> coveredCityIds, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Rabbis
                .Include(r => r.CoveredCities)
                .Include(r => r.HomeCity)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"Rabbi with id {id} was not found.");

            // Same reasoning as CustometRepository.UpdateCustomer: capture what the incoming
            // "rabbi" (mapped straight from RabbiUpdateDTO) does NOT carry, so SetValues can't
            // wipe it. RabbiUpdateDTO never carries Id, PasswordHash or IsActive.
            var realId = existing.Id;
            var realPasswordHash = existing.PasswordHash;
            var realIsActive = existing.IsActive;

            _dataContext.Entry(existing).CurrentValues.SetValues(rabbi);

            existing.Id = realId;
            existing.PasswordHash = realPasswordHash;
            existing.IsActive = realIsActive;

            // Replace the CoveredCities set with exactly what was requested.
            var desiredIds = coveredCityIds.ToHashSet();
            var newCities = await _dataContext.Cities
                .Where(c => desiredIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            existing.CoveredCities.Clear();
            foreach (var city in newCities)
                existing.CoveredCities.Add(city);

            await _dataContext.SaveChangesAsync(cancellationToken);

            // HomeCityId may have changed - make sure the loaded HomeCity nav matches it.
            await _dataContext.Entry(existing).Reference(r => r.HomeCity).LoadAsync(cancellationToken);

            return existing;
        }

        public Task<Rabbi> DeactivateRabbi(int id, CancellationToken cancellationToken)
            => SetActive(id, false, cancellationToken);

        public Task<Rabbi> ReactivateRabbi(int id, CancellationToken cancellationToken)
            => SetActive(id, true, cancellationToken);

        private async Task<Rabbi> SetActive(int id, bool isActive, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Rabbis
                .Include(r => r.HomeCity)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"Rabbi with id {id} was not found.");

            existing.IsActive = isActive;
            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }

        public async Task<Rabbi?> GetRabbiById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Rabbis
                .AsNoTracking()
                .Include(r => r.HomeCity)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Rabbi?> GetRabbiByIdWithOrders(int id, CancellationToken cancellationToken)
        {
            // Everything RabbiAdminDTO/OrderAdminDTO need in one query - no N+1 when the whole
            // graph is mapped afterwards (requirement 7).
            return await _dataContext.Rabbis
                .AsNoTracking()
                .Include(r => r.HomeCity)
                .Include(r => r.HandledOrders).ThenInclude(o => o.City)
                .Include(r => r.HandledOrders).ThenInclude(o => o.RequestedCustomer)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<Rabbi> Items, int TotalCount)> GetAllRabbisPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dataContext.Rabbis.AsNoTracking().Include(r => r.HomeCity).OrderBy(r => r.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Rabbi?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken)
        {
            return await _dataContext.Rabbis
                .AsNoTracking()
                .Include(r => r.HomeCity)
                .FirstOrDefaultAsync(r => r.Phone == phone || r.Phone2 == phone, cancellationToken);
        }

        public async Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken)
        {
            return await _dataContext.Rabbis
                .AsNoTracking()
                .AnyAsync(r => r.Phone == phone || r.Phone2 == phone, cancellationToken);
        }
    }
}
