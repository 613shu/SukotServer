using AutoMapper;
using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.DTOs.RabbiDTOs.Response;
using SukotSystemCore.Exceptions;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using SukotSystemService.Security;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemService
{
    public class RabbiService : IRabbiService
    {
        private readonly IRabbiRepository _rabbiRepository;
        private readonly IMapper _mapper;

        public RabbiService(IRabbiRepository rabbiRepository, IMapper mapper)
        {
            _rabbiRepository = rabbiRepository;
            _mapper = mapper;
        }

        public async Task<RabbiResponseDTO> AddRabbi(RabbiRegisterDTO rabbi, CancellationToken cancellationToken)
        {
            // Business validation belongs here, not the Controller (requirement 2). Only an
            // Admin reaches this action ([Authorize(Roles="Admin")] on the Controller), but the
            // phone still has to be unique within the Rabbi table itself - the same phone is
            // allowed to also exist as a Customer (confirmed design decision).
            if (await _rabbiRepository.PhoneExistsAsync(rabbi.Phone, cancellationToken))
                throw new ConflictException($"A rabbi with phone {rabbi.Phone} already exists.");

            var rabbiMap = _mapper.Map<Rabbi>(rabbi);

            // Password never travels through AutoMapper - hashed explicitly here, same pattern
            // as CustomerService.AddCustomer.
            rabbiMap.PasswordHash = PasswordHasher.Hash(rabbi.Password);

            var res = await _rabbiRepository.AddRabbi(rabbiMap, cancellationToken);

            return _mapper.Map<RabbiResponseDTO>(res);
        }

        public async Task<RabbiResponseDTO> UpdateRabbi(int id, RabbiUpdateDTO rabbi, CancellationToken cancellationToken)
        {
            var rabbiMap = _mapper.Map<Rabbi>(rabbi);

            var updated = await _rabbiRepository.UpdateRabbi(id, rabbiMap, rabbi.CoveredCityIds, cancellationToken);

            return _mapper.Map<RabbiResponseDTO>(updated);
        }

        public async Task<RabbiResponseDTO> DeactivateRabbi(int id, CancellationToken cancellationToken)
        {
            var res = await _rabbiRepository.DeactivateRabbi(id, cancellationToken);
            return _mapper.Map<RabbiResponseDTO>(res);
        }

        public async Task<RabbiResponseDTO> ReactivateRabbi(int id, CancellationToken cancellationToken)
        {
            var res = await _rabbiRepository.ReactivateRabbi(id, cancellationToken);
            return _mapper.Map<RabbiResponseDTO>(res);
        }

        public async Task<RabbiResponseDTO> GetRabbiById(int id, CancellationToken cancellationToken)
        {
            var rabbi = await _rabbiRepository.GetRabbiById(id, cancellationToken);

            if (rabbi == null)
                throw new KeyNotFoundException($"Rabbi with id {id} was not found.");

            return _mapper.Map<RabbiResponseDTO>(rabbi);
        }

        public async Task<RabbiAdminDTO> GetRabbiAdminViewById(int id, CancellationToken cancellationToken)
        {
            var rabbi = await _rabbiRepository.GetRabbiByIdWithOrders(id, cancellationToken);

            if (rabbi == null)
                throw new KeyNotFoundException($"Rabbi with id {id} was not found.");

            return _mapper.Map<RabbiAdminDTO>(rabbi);
        }

        public async Task<PagedResultDTO<RabbiAdminDTO>> GetAllRabbisPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            // Guard against a bad/absent query string producing an invalid query (requirement 2 -
            // exactly the kind of business rule that belongs in the Service, not bound blindly
            // from [FromQuery]). Note: this list view does NOT eager-load HandledOrders (see
            // RabbiRepository.GetAllRabbisPaged) - keeping the roster page light. Orders per
            // rabbi are only loaded on the single-rabbi detail view (GetRabbiAdminViewById).
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var (items, totalCount) = await _rabbiRepository.GetAllRabbisPaged(page, pageSize, cancellationToken);

            return new PagedResultDTO<RabbiAdminDTO>
            {
                Items = _mapper.Map<IEnumerable<RabbiAdminDTO>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
