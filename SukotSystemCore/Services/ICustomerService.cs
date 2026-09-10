using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemCore.Services
{
    public interface ICustomerService
    {
        Task<CustomerResponseDTO> AddCustomer(CustomerRegisterDTO customer, CancellationToken cancellationToken);

        Task<CustomerResponseDTO> UpdateCustomer(int id, CustomerRegisterDTO customer, CancellationToken cancellationToken);
        Task<CustomerResponseDTO> DeleteCustomer(int id, CancellationToken cancellationToken);

        Task<CustomerResponseDTO> GetCustomerById(int id, CancellationToken cancellationToken);

        Task<IEnumerable<CustomerResponseDTO>> GetAllCustomers(CancellationToken cancellationToken);
        Task<CustomerRabbiDTO> GetCustomerRaabiDTOById(int id, CancellationToken cancellationToken);

        Task<CustomerResponseDTO> LoginAsync(LoginModel loginModel, CancellationToken cancellationToken);

    }
}
