using HotelListingAPI.DTO.Hotel;
namespace HotelListingAPI.DTO.Country;

public class GetCountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public List<GetHotelDto> Hotels { get; set; } = new();
};
