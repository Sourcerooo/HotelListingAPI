using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.DTO.Hotel;

public class CreateHotelDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Address { get; set; }

    [Range(0.0, 5.0)]
    public double Rating { get; set; }

    [Required]
    public int CountryId { get; set; }
}