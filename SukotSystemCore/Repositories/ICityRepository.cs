using SukotSystemCore.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Repositories
{
    // City is small reference/lookup data (~194 seeded rows) - every role reads the same shape
    // (Customer picking a city for an Order, Rabbi filtering/browsing by city, Admin managing
    // the list), so unlike Rabbi/Order there is deliberately no paginated list here: the whole
    // point of this endpoint is a dropdown that needs every row at once.
    public interface ICityRepository
    {
        Task<City> AddCity(City city, CancellationToken cancellationToken);
        Task<City> UpdateCity(int id, City city, CancellationToken cancellationToken);

        // Throws ConflictException (translated from the database's own FK-restrict violation)
        // if the city is still referenced by any Rabbi (HomeCity/CoveredCities) or Order.
        Task DeleteCity(int id, CancellationToken cancellationToken);

        Task<City?> GetCityById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<City>> GetAllCities(CancellationToken cancellationToken);

        // excludeId lets UpdateCity check "does this name belong to a DIFFERENT city" instead
        // of always tripping on the row being updated matching its own current name.
        Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken);
    }
}
