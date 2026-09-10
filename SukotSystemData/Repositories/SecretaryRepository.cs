using Microsoft.EntityFrameworkCore;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukotSystemData.Repositories
{
    public class SecretaryRepository : ISecretaryRepository
    {
        private readonly DataContex _dataContext;
        public SecretaryRepository(DataContex dataContex)
        {
            _dataContext= dataContex;   
        }
        public async Task<Secretary> AddSecretary(Secretary secretary, CancellationToken cancellationToken)
        {
            await _dataContext.Secretaries.AddAsync(secretary);
            await _dataContext.SaveChangesAsync(cancellationToken);
            return secretary;
        }

        public async Task<Secretary?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken)
        {
           return await _dataContext.Secretaries.AsNoTracking().FirstOrDefaultAsync( s => s.Phone == phone,cancellationToken);
           
        }

        public async Task<Secretary?> GetSecretaryById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Secretaries.AsNoTracking().FirstOrDefaultAsync(s=>s.Id==id, cancellationToken);
        }

        public async Task<IEnumerable<Secretary>> GetAllSecretaries(CancellationToken cancellationToken)
        {
            return await _dataContext.Secretaries.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
