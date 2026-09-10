using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.DTOs.CustomerDTOs.Request
{
  
    public class CustomerRegisterDTO
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? Email { get; set; }

        [Required]
        public string Phone { get; set; }
        public string? Phone2 { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; } = string.Empty;
    }
}
