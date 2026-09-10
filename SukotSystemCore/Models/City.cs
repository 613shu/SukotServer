using System.Collections.Generic;

namespace SukotSystemCore.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // One-to-many: a City has many Orders (requirement 7).
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        // Many-to-many: Rabbis willing to cover this City (requirement 7).
        public ICollection<Rabbi> Rabbis { get; set; } = new List<Rabbi>();
    }
}
