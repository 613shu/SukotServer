using AutoMapper;
using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.AdminDTOs.Response;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemService
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public async Task<AdminResponseDTO> GetAdminById(int id, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetAdminById(id, cancellationToken);

            if (admin == null)
                throw new KeyNotFoundException($"Admin with id {id} was not found.");

            return _mapper.Map<AdminResponseDTO>(admin);
        }

        public async Task<AdminResponseDTO> UpdateAdmin(int id, AdminUpdateDTO admin, CancellationToken cancellationToken)
        {
            var adminMap = _mapper.Map<Admin>(admin);

            var updated = await _adminRepository.UpdateAdmin(id, adminMap, cancellationToken);

            return _mapper.Map<AdminResponseDTO>(updated);
        }
    }
}
