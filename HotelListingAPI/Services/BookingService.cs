using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingAPI.Constants;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Enums;
using HotelListingAPI.DTO.Booking;
using HotelListingAPI.Results;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace HotelListingAPI.Services;

public class BookingService(HotelListingDbContext context, IUserService userService, IMapper mapper) : IBookingService
{
    public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId, CancellationToken ct = default)
    {
        var hotelExists = await context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotelExists)
        {
            return Result<IEnumerable<GetBookingDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"Hotel with id {hotelId} was not found")
            );
        }
        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId)
            .OrderBy(b => b.CheckInDate)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingsForHotelAsync(int hotelId)
    {
        var userId = userService.UserId;
        var hotelExists = await context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotelExists)
        {
            return Result<IEnumerable<GetBookingDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"Hotel with id {hotelId} was not found")
            );
        }
        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId && b.UserId == userId)
            .OrderBy(b => b.CheckInDate)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto, CancellationToken ct = default)
    {
        var userId = userService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "User is required"));
        }

        var hotel = await context.Hotels
            .Where(h => h.Id == createBookingDto.HotelId)
            .FirstOrDefaultAsync();
        if (hotel is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel with id {createBookingDto.HotelId} was not found"));
        }

        var overlaps = await IsOverlap(createBookingDto.CheckInDate,
            createBookingDto.CheckOutDate,
            createBookingDto.HotelId,
            userId);

        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, $"The selected dates overlap with an existing booking."));
        }

        var nights = createBookingDto.CheckOutDate.DayNumber - createBookingDto.CheckInDate.DayNumber;
        var totalPrice = nights * hotel.PricePerNight;
        var booking = mapper.Map<Booking>(createBookingDto);
        booking.UserId = userId;
        hotel.Bookings.Add(booking);

        await context.SaveChangesAsync();

        var created = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(created);
    }

    private async Task<bool> IsOverlap(DateOnly checkInDate, DateOnly checkOutDate, int hotelId, string userId, int? bookingId = null)
    {
         var query = context.Bookings
                    .Where(b =>
                        b.HotelId == hotelId
                    && b.Status != BookingStatus.Cancelled
                    && checkInDate < b.CheckOutDate
                    && checkOutDate > b.CheckInDate
                    && b.UserId == userId).AsQueryable();
        if(bookingId.HasValue)
        {
            query = query.Where(b => b.Id != bookingId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto)
    {
        var userId = userService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, "User is required"));
        }

        var hotel = await context.Hotels
            .Where(h => h.Id == hotelId)
            .FirstOrDefaultAsync();
        if (hotel is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel with id {hotelId} was not found"));
        }

        var booking = await context.Bookings
            .Where(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId)
            .FirstOrDefaultAsync();
        if (booking is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"Booking with id {bookingId} was not found for hotel {hotelId}"));
        }
        if (booking.Status == BookingStatus.Cancelled)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Cannot update a cancelled booking."));
        }

        var overlaps = await IsOverlap(updateBookingDto.CheckInDate,
            updateBookingDto.CheckOutDate,
            hotelId,
            userId,
            bookingId);
        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, $"The selected dates overlap with an existing booking."));
        }

        var nights = updateBookingDto.CheckOutDate.DayNumber - updateBookingDto.CheckInDate.DayNumber;
        var totalPrice = nights * hotel.PricePerNight;

        mapper.Map(updateBookingDto, booking);
        booking.UpdatedAtUtc = DateTime.UtcNow;
        booking.TotalPrice = totalPrice;

        await context.SaveChangesAsync();

        var created = mapper.Map<GetBookingDto>(booking);

        return Result<GetBookingDto>.Success(created);
    }

    public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = userService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, "User is required"));
        }

        var hotel = await context.Hotels
            .Where(h => h.Id == hotelId)
            .FirstOrDefaultAsync();
        if (hotel is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Hotel with id {hotelId} was not found"));
        }

        var booking = await context.Bookings
            .Where(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId)
            .FirstOrDefaultAsync();
        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking with id {bookingId} was not found for hotel {hotelId}"));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == hotelId)
            .FirstOrDefaultAsync();
        if (hotel is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Hotel with id {hotelId} was not found"));
        }

        var booking = await context.Bookings
            .Where(b => b.Id == bookingId && b.HotelId == hotelId)
            .FirstOrDefaultAsync();
        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking with id {bookingId} was not found for hotel {hotelId}"));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == hotelId)
            .FirstOrDefaultAsync();
        if (hotel is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Hotel with id {hotelId} was not found"));
        }

        var booking = await context.Bookings
            .Where(b => b.Id == bookingId && b.HotelId == hotelId)
            .FirstOrDefaultAsync();
        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"Booking with id {bookingId} was not found for hotel {hotelId}"));
        }

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }
}
