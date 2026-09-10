using SukotSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Repositories
{
    public interface ICustomerRepository
    {

        Task<Customer> AddCustomer(Customer customer, CancellationToken cancellationToken);

        Task<Customer> UpdateCustomer(int id, Customer customer, CancellationToken cancellationToken);
        Task<Customer> DeleteCustomer(int id, CancellationToken cancellationToken);

        Task<Customer?> GetCustomerById(int id, CancellationToken cancellationToken);

        Task<IEnumerable<Customer>> GetAllCustomers(CancellationToken cancellationToken);

        // Used for phone+password login. Matches either registered phone number (Phone or Phone2).
        // Returns null when nothing matches - the Service layer decides what that means.
        Task<Customer?> GetByPhoneForLoginAsync(string phone, CancellationToken cancellationToken);
    }
}
