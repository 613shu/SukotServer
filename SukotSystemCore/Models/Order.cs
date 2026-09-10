using System;
using System.ComponentModel.DataAnnotations;
using SukotSystemCore.Enums;

namespace SukotSystemCore.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int ?CustomerId { get; set; }
        public Customer? RequestedCustomer { get; set; }

        public string? CallerPhone { get; set; }
        public string? CallerFullName { get; set; }
        public int? PerformedRabbiId { get; set; }
        public Rabbi? PerformedRabbi { get; set; }

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public DateTime RequestedDate { get; set; }
        public DateTime? PerformedDate { get; set; }
        public bool NeedsTool { get; set; }
        public string? Commits { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public string Adress { get; set; } = string.Empty;

        // Part C - optimistic concurrency token (Appendix D, PostgreSQL: self-managed approach,
        // confirmed with the user over the built-in xmin column). Replaced on every save by
        // DataContex.SaveChangesAsync's override, never by hand anywhere else. EF Core adds the
        // ORIGINAL (as-loaded) value of this field to the UPDATE statement's WHERE clause - if
        // another request already changed the row in between, no row matches, zero rows are
        // affected, and EF throws DbUpdateConcurrencyException (-> 409 via
        // ExceptionHandlingMiddleware). Never exposed to/accepted from the client - it does not
        // appear on any DTO.
        [ConcurrencyCheck]
        public Guid Version { get; set; } = Guid.NewGuid();
    }
}
