using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.DTOs.RabbiDTOs.Request
{
    // Mirrors CustomerRegisterDTO's shape/conventions for the Rabbi's own registration.
    public class RabbiRegisterDTO
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }

        [Required]
        public int HomeCityId { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; } = string.Empty;
    }
}
