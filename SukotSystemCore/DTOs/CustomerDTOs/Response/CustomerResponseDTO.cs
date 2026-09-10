namespace SukotSystemCore.DTOs.CustomerDTOs.Response
{
   
    public class CustomerResponseDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public string? Phone2 { get; set; }
    }
}
