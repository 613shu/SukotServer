using SukotSystemCore.DTOs.SecretaryDTOs;
using SukotSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    public interface ISecreratyService
    {
        Task<SecretaryResponseDTO> AddSecretary(SecretaryRequstDTO secretary, CancellationToken cancellationToken);


        Task<SecretaryResponseDTO?> GetSecretaryById(int id, CancellationToken cancellationToken);


        Task<SecretaryResponseDTO?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken);

        // Admin-only roster listing.
        Task<IEnumerable<SecretaryResponseDTO>> GetAllSecretaries(CancellationToken cancellationToken);

    }
}
