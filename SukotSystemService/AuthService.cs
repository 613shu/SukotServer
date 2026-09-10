using AutoMapper;
using Microsoft.Extensions.Logging;
using SukotSystemCore.DTOs.AdminDTOs.Response;
using SukotSystemCore.DTOs.Common;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs.Response;
using SukotSystemCore.DTOs.SecretaryDTOs;
using SukotSystemCore.Models;
using SukotSystemCore.Repositories;
using SukotSystemCore.Services;
using SukotSystemService.Security;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SukotSystemService
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IRabbiRepository _rabbiRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISecretaryRepository _secreratyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAdminRepository adminRepository,
            IRabbiRepository rabbiRepository,
            ICustomerRepository customerRepository,
            ISecretaryRepository secreratyRepository,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _adminRepository = adminRepository;
            _rabbiRepository = rabbiRepository;
            _customerRepository = customerRepository;
            _secreratyRepository=secreratyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // A single phone number is checked against the three role tables in a fixed priority
        // order - Admin, then Rabbi, then Customer - and whichever table it is found in FIRST
        // "owns" that login: the password is verified against that table's own hash, and the
        // search stops there (it never falls through to try the same phone against a
        // lower-priority table's password). The one exception is the inactive-rabbi rule below,
        // which was explicitly confirmed with the user.
        public async Task<LoginResultDTO> LoginAsync(LoginModel loginModel, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByPhoneForLoginAsync(loginModel.Phone, cancellationToken);
            if (admin != null)
            {
                
                if (!PasswordHasher.Verify(loginModel.Password, admin.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid password.");

                return BuildResult("Admin", admin.Id, admin.FullName, admin.Phone,
                    adminProfile: _mapper.Map<AdminResponseDTO>(admin));
            }

            var rabbi = await _rabbiRepository.GetByPhoneForLoginAsync(loginModel.Phone, cancellationToken);
            if (rabbi != null)
            {
                if (!PasswordHasher.Verify(loginModel.Password, rabbi.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid  password.");

                if (rabbi.IsActive)
                {
                    return BuildResult("Rabbi", rabbi.Id, $"{rabbi.FirstName} {rabbi.LastName}", rabbi.Phone,
                        rabbiProfile: _mapper.Map<RabbiResponseDTO>(rabbi));
                }

                // Confirmed design decision: the same phone number can also be registered as a
                // Customer (two separate rows, same person). If so, an inactive Rabbi logs in as
                // that Customer instead of as the (now inactive) Rabbi. The Rabbi credentials
                // just verified above are what authenticated them - there is no second password
                // check against the Customer row.
                var linkedCustomer = await _customerRepository.GetByPhoneForLoginAsync(loginModel.Phone, cancellationToken);
                if (linkedCustomer == null)
                {
                    _logger.LogWarning(
                        "Inactive rabbi {RabbiId} (phone {Phone}) attempted login but has no linked customer profile.",
                        rabbi.Id, rabbi.Phone);
                    throw new UnauthorizedAccessException(
                        "This rabbi account is no longer active, and no customer profile exists for this phone number.");
                }

                _logger.LogWarning(
                    "Inactive rabbi {RabbiId} (phone {Phone}) logged in as customer {CustomerId} instead.",
                    rabbi.Id, rabbi.Phone, linkedCustomer.Id);

                return BuildResult("Customer", linkedCustomer.Id, $"{linkedCustomer.FirstName} {linkedCustomer.LastName}", linkedCustomer.Phone,
                    customerProfile: _mapper.Map<CustomerResponseDTO>(linkedCustomer));
            }
            var secretary = await _secreratyRepository.GetByPhoneForLoginAsync(loginModel.Phone, cancellationToken);
            if (secretary != null)
            {
                if (!PasswordHasher.Verify(loginModel.Password, secretary.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid phone or password.");

                return BuildResult("Secretary", secretary.Id, "מזכירה", secretary.Phone,
                   secretaryProfile: _mapper.Map<SecretaryResponseDTO>(secretary));

            }
            var customer = await _customerRepository.GetByPhoneForLoginAsync(loginModel.Phone, cancellationToken);
            if (customer != null)
            {
                if (!PasswordHasher.Verify(loginModel.Password, customer.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid phone or password.");

                return BuildResult("Customer", customer.Id, $"{customer.FirstName} {customer.LastName}", customer.Phone,
                    customerProfile: _mapper.Map<CustomerResponseDTO>(customer));
            }

            throw new KeyNotFoundException($"No account registered with phone {loginModel.Phone}.");
        }

        private static LoginResultDTO BuildResult(
            string role, int userId, string displayName, string phone,
            AdminResponseDTO? adminProfile = null,
            RabbiResponseDTO? rabbiProfile = null,
            SecretaryResponseDTO? secretaryProfile=null,
            CustomerResponseDTO? customerProfile = null
            )
        {
            return new LoginResultDTO
            {
                Role = role,
                UserId = userId,
                DisplayName = displayName,
                Phone = phone,
                AdminProfile = adminProfile,
                RabbiProfile = rabbiProfile,
                SecretaryProfile=secretaryProfile,
                CustomerProfile = customerProfile
            };
        }
    }
}
