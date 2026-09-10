namespace SukotSystemCore.DTOs.Common
{
    // City is not audience-sensitive lookup data - every role (Customer picking a city for an
    // order, Rabbi filtering/browsing by city, Admin managing the list) can see the same shape,
    // so it lives in Common rather than being duplicated per role folder.
    public class CityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
