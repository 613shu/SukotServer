using SukotSystemCore.Enums;
using System;

namespace SukotSystemCore.DTOs.CustomerDTOs.Response
{
    // A Customer's own view of one of their orders. Never exposes the Customer or Rabbi
    // entities directly (requirement 8) - just flat scalar fields and the related names.
    public class OrderResponseDTO
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

        public int? PerformedRabbiId { get; set; }
        public string? PerformedRabbiName { get; set; }
    }
}
