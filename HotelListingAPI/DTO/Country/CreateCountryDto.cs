using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.DTO.Country
{
    public class CreateCountryDto
    {
        [Required]
        [MaxLength(50)]
        required public string Name { get; set; }

        [Required]
        [MaxLength(3)]
        required public string ShortName { get; set; }
    }
}
