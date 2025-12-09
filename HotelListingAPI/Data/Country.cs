namespace HotelListingAPI.Data;

public class Country
{
    public int CountryId { get; set; }
    required public string Name { get; set; }
    required public string ShortName {  get; set; }
    public IList<Hotel> Hotels { get; set; } = [];

}