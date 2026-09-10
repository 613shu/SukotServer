using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.DTOs.AdminDTOs.Request
{
    // Only an Admin creates/manages the City lookup list.
    public class CityCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
