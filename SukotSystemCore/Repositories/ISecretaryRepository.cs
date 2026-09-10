using SukotSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukotSystemCore.Repositories
{
    public interface ISecretaryRepository
    {
        Task<Secretary> AddSecretary(Secretary secretary, CancellationToken cancellationToken);


        Task<Secretary?> GetSecretaryById(int id, CancellationToken cancellationToken);

  
        // Used for phone+password login. Matches either registered phone number (Phone or Phone2).
        Task<Secretary?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken);

        // Admin-only roster listing - Secretary is small, admin-managed data (same reasoning as
        // City), so no pagination here, consistent with that precedent.
        Task<IEnumerable<Secretary>> GetAllSecretaries(CancellationToken cancellationToken);

    }
}
