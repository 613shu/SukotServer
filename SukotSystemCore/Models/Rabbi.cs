using System.Collections.Generic;

namespace SukotSystemCore.Models
{
    public class Rabbi
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }
        public string? PasswordHash { get; set; }

        // The Rabbi's primary city (separate from the many-to-many coverage list below).
        public int HomeCityId { get; set; }
        public City HomeCity { get; set; } = null!;

        // Set/cleared by an Admin (managing the rabbi roster).
        public bool IsActive { get; set; } = true;

        // Many-to-many: other cities this Rabbi is willing to travel to and cover,
        // in addition to HomeCity (requirement 7's many-to-many relationship).
        public ICollection<City> CoveredCities { get; set; } = new List<City>();

        // One-to-many inverse: Orders this Rabbi has claimed/performed.
        public ICollection<Order> HandledOrders { get; set; } = new List<Order>();
    }
}
