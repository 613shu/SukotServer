using SukotSystemCore.Enums;
using System;

namespace SukotSystemCore.DTOs.AdminDTOs
{
    // An Admin's system-wide view of an Order: who requested it, who (if anyone) is handling
    // it, and when. No entities exposed directly (requirement 8) - just DTOs and flat fields.
    public class OrderAdminDTO
    {
        public int Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? PerformedDate { get; set; }
        public bool NeedsTool { get; set; }
        public string? Commits { get; set; }
        public OrderStatus Status { get; set; }
        public string Adress { get; set; } = string.Empty;

        // Present only for orders taken over the phone by a Secretary on behalf of a caller who
        // never registered as a Customer (RequestedCustomer/CustomerId are null in that case) -
        // without these, an Admin/Secretary reviewing the order list would have no way to see who
        // the caller was at all.
        public string? CallerFullName { get; set; }
        public string? CallerPhone { get; set; }

        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;

        public CustomerAdminDTO RequestedCustomer { get; set; } 
        public RabbiAdminDTO? RabbInChargeOfThisOrder { get; set; }

        public int? PerformedRabbiId { get; set; }
        public string? PerformedRabbiName { get; set; }
    }
}
