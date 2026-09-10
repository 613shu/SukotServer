using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.Models
{
    // Login is now phone + password only.
    // The phone can match either of the customer's two registered phone numbers (Phone or Phone2).
    public class LoginModel
    {
        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
