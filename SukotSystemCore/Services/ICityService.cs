using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    public interface ICityService
    {
        Task<CityDTO> AddCity(CityCreateDTO city, CancellationToken cancellationToken);

        // CityCreateDTO is reused for updates too (its one field, Name, is the whole shape of
        // both operations) - the same "Update" naming split as Rabbi/Admin, just without a
        // separate DTO type, since there is nothing else to protect from being overwritten.
        Task<CityDTO> UpdateCity(int id, CityCreateDTO city, CancellationToken cancellationToken);

        Task DeleteCity(int id, CancellationToken cancellationToken);

        Task<CityDTO> GetCityById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<CityDTO>> GetAllCities(CancellationToken cancellationToken);
    }
}
