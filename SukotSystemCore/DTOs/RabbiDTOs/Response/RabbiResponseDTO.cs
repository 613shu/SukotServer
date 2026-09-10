namespace SukotSystemCore.DTOs.RabbiDTOs.Response
{
    // A Rabbi's own profile view after login/registration. Never contains PasswordHash.
    public class RabbiResponseDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Phone2 { get; set; }

        public int HomeCityId { get; set; }
        public string HomeCityName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
