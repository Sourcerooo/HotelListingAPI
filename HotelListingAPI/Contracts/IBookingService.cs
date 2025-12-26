using HotelListingAPI.DTO.Booking;
using HotelListingAPI.Results;

namespace HotelListingAPI.Contracts
{
    public interface IBookingService
    {
        Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);
        Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
        Task<Result> CancelBookingAsync(int hotelId, int bookingId);
        Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto, CancellationToken ct = default);
        Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId, CancellationToken ct = default);
        Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingsForHotelAsync(int hotelId);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
    }
}