using SukotSystemCore.Enums;
using System;

namespace SukotSystemCore.DTOs.RabbiDTOs
{
    // An Order as a Rabbi is allowed to see it - when browsing unclaimed requests, or looking
    // at one already claimed. Nests CustomerRabbiDTO (a DTO, not the Customer entity) so the
    // rabbi gets contact details without the API ever exposing the full Customer/City/Rabbi
    // entities (requirement 8).
    public class OrderRabbiDTO
    {
        public int Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? PerformedDate { get; set; }
        public bool NeedsTool { get; set; }
        public string? Commits { get; set; }
        public OrderStatus Status { get; set; }
        public string Adress { get; set; } = string.Empty;

        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;

        public CustomerRabbiDTO RequestedCustomer { get; set; } 
    }
}
