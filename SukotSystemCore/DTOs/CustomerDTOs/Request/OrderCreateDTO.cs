using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.DTOs.CustomerDTOs.Request
{
    // What a Customer supplies when submitting a new inspection request.
    // Deliberately excludes: Id (server-assigned), CustomerId (comes from the JWT, never from
    // the client), Status (server always starts it at New), and PerformedRabbi/PerformedDate
    // (set later, only by the claim flow).
    public class OrderCreateDTO
    {
        public string? CallerFullName
        { get; set; }

        public string ?CallerPhone { get; set; }
        
        [Required]
        public int CityId { get; set; }

        [Required]
        public string Adress { get; set; } = string.Empty;

        public bool NeedsTool { get; set; }
    }
}
