using System.Collections.Generic;

namespace SukotSystemCore.DTOs.AdminDTOs
{
    // An Admin's view of a Rabbi for roster management (add/remove - per Part A's role
    // description). Includes IsActive, which only an Admin should be able to see/set, and the
    // full list of Orders this Rabbi has handled, so an Admin can review one inspector's
    // history in one place.
    public class RabbiAdminDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }
        public string? Email { get; set; }

        public int HomeCityId { get; set; }
        public string HomeCityName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        // All orders this Rabbi has claimed/performed, for the Admin to review.
        public ICollection<OrderAdminDTO> Orders { get; set; } = new List<OrderAdminDTO>();
    }
}
