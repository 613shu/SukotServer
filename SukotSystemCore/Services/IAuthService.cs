using SukotSystemCore.DTOs.Common;
using SukotSystemCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    // The one place that knows how to resolve a single phone+password login across all three
    // role tables (Admin/Rabbi/Customer) and decide which role the caller actually gets.
    public interface IAuthService
    {
        Task<LoginResultDTO> LoginAsync(LoginModel loginModel, CancellationToken cancellationToken);
    }
}
