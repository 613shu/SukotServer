using Microsoft.EntityFrameworkCore;
using Npgsql;
using SukotSystemCore.Exceptions;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemData
{
    public class CityRepository : ICityRepository
    {
        private readonly DataContex _dataContext;

        public CityRepository(DataContex dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<City> AddCity(City city, CancellationToken cancellationToken)
        {
            await _dataContext.Cities.AddAsync(city, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return city;
        }

        public async Task<City> UpdateCity(int id, City city, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Cities.FindAsync(new object[] { id }, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"City with id {id} was not found.");

            // Same reasoning as AdminRepository.UpdateAdmin/RabbiRepository.UpdateRabbi: the
            // incoming "city" (mapped straight from CityCreateDTO) never carries Id, so
            // SetValues would otherwise zero it out.
            var realId = existing.Id;
            _dataContext.Entry(existing).CurrentValues.SetValues(city);
            existing.Id = realId;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }

        public async Task DeleteCity(int id, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Cities.FindAsync(new object[] { id }, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"City with id {id} was not found.");

            _dataContext.Cities.Remove(existing);

            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
            {
                // A Rabbi (HomeCity or CoveredCities) or an Order still references this City -
                // every one of those relationships is DeleteBehavior.Restrict on purpose (see
                // DataContex.OnModelCreating). Relying on the database's own constraint here
                // instead of a separate "is it in use" pre-check query avoids a race between
                // the check and the delete - the same spirit as Part C, just for a DELETE
                // instead of an UPDATE.
                throw new ConflictException("This city is still in use by a rabbi or a request and cannot be deleted.");
            }
        }

        public async Task<City?> GetCityById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Cities
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<City>> GetAllCities(CancellationToken cancellationToken)
        {
            return await _dataContext.Cities
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken)
        {
            return await _dataContext.Cities
                .AsNoTracking()
                .AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId), cancellationToken);
        }

        private static bool IsForeignKeyViolation(DbUpdateException ex)
            => ex.InnerException is PostgresException pg && pg.SqlState == "23503";
    }
}
