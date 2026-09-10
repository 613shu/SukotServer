using AutoMapper;
using SukotSystemCore.DTOs.CustomerDTOs.Request;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using SukotSystemService.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SukotSystemService
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<CustomerResponseDTO> AddCustomer(CustomerRegisterDTO customer, CancellationToken cancellationToken)
        {

            var customerMap = _mapper.Map<Customer>(customer);

            // Password never travels through AutoMapper (property names differ on purpose).
            // Hash it explicitly here, in the Service layer, before it is persisted.
            customerMap.PasswordHash = PasswordHasher.Hash(customer.Password);

            var res = await _customerRepository.AddCustomer(customerMap, cancellationToken);

            return _mapper.Map<CustomerResponseDTO>(res);
        }

        public async Task<CustomerResponseDTO> DeleteCustomer(int id, CancellationToken cancellationToken)
        {
            Customer customer= await _customerRepository.DeleteCustomer(id, cancellationToken);
            var customerMap = _mapper.Map<CustomerResponseDTO>(customer);

            return customerMap;
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllCustomers(CancellationToken cancellationToken)
        {
            var res = await _customerRepository.GetAllCustomers(cancellationToken);

            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(res);
        }

        public async Task<CustomerResponseDTO> GetCustomerById(int id, CancellationToken cancellationToken)
        {

                var customer = await _customerRepository.GetCustomerById(id, cancellationToken);

                if (customer == null)
                    throw new KeyNotFoundException($"Customer with id {id} was not found.");

                return _mapper.Map<CustomerResponseDTO>(customer);

        }

        public async Task<CustomerRabbiDTO> GetCustomerRaabiDTOById(int id, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetCustomerById(id, cancellationToken);

            if (customer == null)
                throw new KeyNotFoundException($"Customer with id {id} was not found.");


            return _mapper.Map<CustomerRabbiDTO>(customer);
        }


        public async Task<CustomerResponseDTO> LoginAsync(LoginModel loginModel, CancellationToken cancellationToken)
        {
            // Matches on either registered phone number (Phone or Phone2) - see CustometRepository.GetByPhoneForLoginAsync.
            var customer = await _customerRepository.GetByPhoneForLoginAsync(loginModel.Phone, cancellationToken);

            if (customer == null)
                throw new KeyNotFoundException($"No customer registered with phone {loginModel.Phone}.");

            if (!PasswordHasher.Verify(loginModel.Password, customer.PasswordHash))
                throw new UnauthorizedAccessException("Invalid phone or password.");

            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<CustomerResponseDTO> UpdateCustomer(int id, CustomerRegisterDTO customer, CancellationToken cancellationToken)
        {
            var customerMap = _mapper.Map<Customer>(customer);


            var updated = await _customerRepository.UpdateCustomer(id, customerMap, cancellationToken);

            return _mapper.Map<CustomerResponseDTO>(updated);
        }
    }
}
