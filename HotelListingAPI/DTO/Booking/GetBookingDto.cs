namespace HotelListingAPI.DTO.Booking;

public record GetBookingDto(
    int Id,
    int HotelId,
    string HotelName,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    int NumberOfGuests,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);
