using AutoMapper;
using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.Exceptions;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemService
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityService(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<CityDTO> AddCity(CityCreateDTO city, CancellationToken cancellationToken)
        {
            // Business validation (state-dependent, so it belongs here and not in the
            // Controller/DataAnnotations) - a duplicate city name is a 409, not a 400.
            if (await _cityRepository.NameExistsAsync(city.Name, null, cancellationToken))
                throw new ConflictException($"A city named '{city.Name}' already exists.");

            var cityMap = _mapper.Map<City>(city);
            var added = await _cityRepository.AddCity(cityMap, cancellationToken);

            return _mapper.Map<CityDTO>(added);
        }

        public async Task<CityDTO> UpdateCity(int id, CityCreateDTO city, CancellationToken cancellationToken)
        {
            if (await _cityRepository.NameExistsAsync(city.Name, id, cancellationToken))
                throw new ConflictException($"A city named '{city.Name}' already exists.");

            var cityMap = _mapper.Map<City>(city);
            var updated = await _cityRepository.UpdateCity(id, cityMap, cancellationToken);

            return _mapper.Map<CityDTO>(updated);
        }

        public async Task DeleteCity(int id, CancellationToken cancellationToken)
        {
            await _cityRepository.DeleteCity(id, cancellationToken);
        }

        public async Task<CityDTO> GetCityById(int id, CancellationToken cancellationToken)
        {
            var city = await _cityRepository.GetCityById(id, cancellationToken);

            if (city == null)
                throw new KeyNotFoundException($"City with id {id} was not found.");

            return _mapper.Map<CityDTO>(city);
        }

        public async Task<IEnumerable<CityDTO>> GetAllCities(CancellationToken cancellationToken)
        {
            var cities = await _cityRepository.GetAllCities(cancellationToken);

            return _mapper.Map<IEnumerable<CityDTO>>(cities);
        }
    }
}
