using SukotSystemCore.DTOs.AdminDTOs.Request;
using SukotSystemCore.DTOs.AdminDTOs.Response;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    public interface IAdminService
    {
        Task<AdminResponseDTO> GetAdminById(int id, CancellationToken cancellationToken);
        Task<AdminResponseDTO> UpdateAdmin(int id, AdminUpdateDTO admin, CancellationToken cancellationToken);
    }
}
