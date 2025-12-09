using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.DTO.Country
{
    public class UpdateCountryDto : CreateCountryDto
    {
        [Required]
        public int Id { get; set; }
    }
}
