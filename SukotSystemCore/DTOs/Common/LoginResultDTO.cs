using SukotSystemCore.DTOs.AdminDTOs.Response;
using SukotSystemCore.DTOs.CustomerDTOs.Response;
using SukotSystemCore.DTOs.RabbiDTOs.Response;
using SukotSystemCore.DTOs.SecretaryDTOs;

namespace SukotSystemCore.DTOs.Common
{
    // What a successful /auth/login resolves to, whatever table the phone number matched.
    // Exactly one of the three profile properties is populated - AuthHelper.CreateToken reads
    // Role to know which claims to issue, and the client can rely on the same rule to know
    // which profile shape it got back.
    public class LoginResultDTO
    {
        public string Role { get; set; } = string.Empty; // "Admin" | "Rabbi" | "Customer"
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public AdminResponseDTO? AdminProfile { get; set; }
        public RabbiResponseDTO? RabbiProfile { get; set; }
        public CustomerResponseDTO? CustomerProfile { get; set; }
        public SecretaryResponseDTO? SecretaryProfile {get;set; }
    }
}
