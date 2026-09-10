namespace SukotSystemCore.DTOs.AdminDTOs
{
    // An Admin's full view of a Customer.
    public class CustomerAdminDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
    }
}
