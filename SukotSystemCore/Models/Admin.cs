namespace SukotSystemCore.Models
{
    // Super-admin (מנהל-על). Per the project domain there are exactly three fixed/seeded
    // accounts - no public self-registration endpoint is expected for this entity.
    public class Admin
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
    }
}
