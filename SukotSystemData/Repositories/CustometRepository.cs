using Microsoft.EntityFrameworkCore;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemData.Repositories
{
    public class CustometRepository : ICustomerRepository
    {
        private readonly DataContex _dataContext;

        public CustometRepository(DataContex dataContex)
        {
            _dataContext = dataContex;

        }
        public async Task<Customer> AddCustomer(Customer customer, CancellationToken cancellationToken)
        {
            await _dataContext.Customers.AddAsync(customer, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);


            return customer;
        }

        public async Task<Customer> DeleteCustomer(int id, CancellationToken cancellationToken)
        {

            var customer = await _dataContext.Customers.FindAsync(new object[] { id }, cancellationToken);

            if (customer == null)
                throw new KeyNotFoundException($"Customer with id {id} was not found.");

            _dataContext.Customers.Remove(customer);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return customer;

        }

        public async Task<IEnumerable<Customer>> GetAllCustomers(CancellationToken cancellationToken)
        {
            return await _dataContext.Customers.ToListAsync(cancellationToken);
        }

        public async Task<Customer?> GetCustomerById(int id, CancellationToken cancellationToken)
        {
            Customer customer =
                await _dataContext.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            return customer;
        }



        public async Task<Customer> UpdateCustomer(int id, Customer customer, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Customers.FindAsync(new object[] { id }, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"Customer with id {id} was not found.");

            // The Request DTO that "customer" was mapped from carries neither Id nor PasswordHash
            // (see CustomerRegisterDTO), so both arrive here as default(0)/null. Capture the real
            // values before SetValues overwrites them.
            var realId = existing.Id;
            var realPasswordHash = existing.PasswordHash;

            _dataContext.Entry(existing).CurrentValues.SetValues(customer);

            // A profile update is not a request to move this row (Id) or to clear the password
            // (PasswordHash) - restore both. A dedicated change-password flow, if one is added
            // later, should update PasswordHash explicitly and go through nothing else.
            existing.Id = realId;
            existing.PasswordHash = realPasswordHash;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }

        // Login lookup: read-only, so AsNoTracking; matches either of the two registered phones.
        // Returns null (rather than throwing) so the Service layer owns the "not found" decision.
        public async Task<Customer?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken)
        {
            return await _dataContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Phone == phone || c.Phone2 == phone, cancellationToken);
        }

    }
}
