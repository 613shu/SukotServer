using AutoMapper;
using SukotSystemCore.DTOs.SecretaryDTOs;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using SukotSystemService.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukotSystemService
{
    public class SecretaryService : ISecreratyService
    {
        private readonly ISecretaryRepository _secretaryRepository;
        private readonly IMapper _mapper;

        public SecretaryService(ISecretaryRepository secretaryRepository,IMapper mapper)
        {
            _secretaryRepository = secretaryRepository;
            _mapper = mapper;
        }
        public async Task<SecretaryResponseDTO> AddSecretary(SecretaryRequstDTO secretary, CancellationToken cancellationToken)
        {
            Secretary s=_mapper.Map<Secretary>(secretary);
            s.PasswordHash = PasswordHasher.Hash(secretary.Password);
            var res= await _secretaryRepository.AddSecretary(s, cancellationToken);
            return _mapper.Map<SecretaryResponseDTO>(res);

        }

        public async Task<SecretaryResponseDTO?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken)
        {
           var res=await _secretaryRepository.GetByPhoneForLoginAsync(phone, cancellationToken);
            return _mapper.Map<SecretaryResponseDTO>(res);
        }

        public async Task<SecretaryResponseDTO?> GetSecretaryById(int id, CancellationToken cancellationToken)
        {
           var res= await _secretaryRepository.GetSecretaryById(id, cancellationToken);
            return _mapper.Map<SecretaryResponseDTO>(res);

        }

        public async Task<IEnumerable<SecretaryResponseDTO>> GetAllSecretaries(CancellationToken cancellationToken)
        {
            var res = await _secretaryRepository.GetAllSecretaries(cancellationToken);
            return _mapper.Map<IEnumerable<SecretaryResponseDTO>>(res);
        }
    }
}
