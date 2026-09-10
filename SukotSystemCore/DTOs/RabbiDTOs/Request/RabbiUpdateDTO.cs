using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.DTOs.RabbiDTOs.Request
{
    // Shared body for both "a Rabbi updates their own profile" (PUT /rabbis/me) and "an Admin
    // edits a Rabbi's details" (PUT /rabbis/{id}). Deliberately excludes: Id, PasswordHash, and
    // IsActive - IsActive only ever changes through the dedicated deactivate/reactivate actions,
    // never through a general profile edit (same reasoning as Customer's UpdateCustomer never
    // touching PasswordHash).
    public class RabbiUpdateDTO
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Email { get; set; }

        [Required]
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }

        [Required]
        public int HomeCityId { get; set; }

        // Full replacement list of cities (besides HomeCity) this Rabbi is willing to cover.
        public ICollection<int> CoveredCityIds { get; set; } = new List<int>();
    }
}
