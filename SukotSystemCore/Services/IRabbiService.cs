using SukotSystemCore.DTOs.AdminDTOs;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.RabbiDTOs.Request;
using SukotSystemCore.DTOs.RabbiDTOs.Response;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    public interface IRabbiService
    {
        // Admin-only: adds a new Rabbi to the roster (Part A - only an Admin adds/removes rabbis).
        Task<RabbiResponseDTO> AddRabbi(RabbiRegisterDTO rabbi, CancellationToken cancellationToken);

        // Shared shape for both "a Rabbi edits their own profile" (/rabbis/me) and "an Admin
        // edits a Rabbi's details" (/rabbis/{id}) - the Controller enforces who is allowed to
        // target which id; the Service just performs the update.
        Task<RabbiResponseDTO> UpdateRabbi(int id, RabbiUpdateDTO rabbi, CancellationToken cancellationToken);

        Task<RabbiResponseDTO> DeactivateRabbi(int id, CancellationToken cancellationToken);
        Task<RabbiResponseDTO> ReactivateRabbi(int id, CancellationToken cancellationToken);

        Task<RabbiResponseDTO> GetRabbiById(int id, CancellationToken cancellationToken);
        Task<RabbiAdminDTO> GetRabbiAdminViewById(int id, CancellationToken cancellationToken);

        Task<PagedResultDTO<RabbiAdminDTO>> GetAllRabbisPaged(int page, int pageSize, CancellationToken cancellationToken);
    }
}
