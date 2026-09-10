namespace SukotSystemCore.DTOs.AdminDTOs.Response
{
    // An Admin's own profile view after login. Never contains PasswordHash.
    public class AdminResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
