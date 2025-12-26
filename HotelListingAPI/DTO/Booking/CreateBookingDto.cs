using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.DTO.Booking;

public record CreateBookingDto(
    [Required] int HotelId,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    [Required][Range(minimum: 1, maximum: 10)] int NumberOfGuests
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "Check-out date must be later than check-in date.",
                [ nameof(CheckOutDate), nameof(CheckInDate) ]
            );
        }
    }
}