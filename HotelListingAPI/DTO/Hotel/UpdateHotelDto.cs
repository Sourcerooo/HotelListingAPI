using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.DTO.Hotel;

public class UpdateHotelDto : CreateHotelDto
{
    [Required]
    public required int Id { get; set; }
}

