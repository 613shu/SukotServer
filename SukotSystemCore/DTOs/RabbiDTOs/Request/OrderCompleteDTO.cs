namespace SukotSystemCore.DTOs.RabbiDTOs.Request
{
    // Body for PATCH /orders/{id}/complete - the rabbi who claimed the order records the
    // outcome of the inspection. Everything else about the transition (Status -> Completed,
    // PerformedDate is already set from the claim) is decided server-side, not by the client.
    public class OrderCompleteDTO
    {
        public string? Commits { get; set; }
    }
}
