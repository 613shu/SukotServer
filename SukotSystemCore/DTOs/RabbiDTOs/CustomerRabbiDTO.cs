namespace SukotSystemCore.DTOs.RabbiDTOs
{
    // What a Rabbi is allowed to see about the Customer behind a request - contact info only,
    // no PasswordHash, no Email exposure beyond what's already public-facing here.
    public class CustomerRabbiDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }
    }
}
