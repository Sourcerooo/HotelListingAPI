using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingAPI.Constants;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.DTO.Hotel;
using HotelListingAPI.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Services;

public class HotelService(HotelListingDbContext context,
    ICountryService countryService,
    IMapper mapper) : IHotelService
{
    public async Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync()
    {
        var hotels = await context.Hotels
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return Result<IEnumerable<GetHotelDto>>.Success(hotels);
    }

    public async Task<Result<GetHotelDto>> GetHotelAsync(int id)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if(hotel is null)
        {
            return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel with id {id} was not found"));
        }
        return Result<GetHotelDto>.Success(hotel);
    }

    public async Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateEntry)
    {
        if (id != updateEntry.Id)
        {
            return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Id route value does not match Payload Id"));
        }

        var hotel = await context.Hotels.FindAsync(updateEntry.Id);
        if(hotel is null)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel with id {id} was not found"));
        }

        var countryExists = await countryService.CountryExistsAsync(updateEntry.CountryId);
        if (!countryExists)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with id {updateEntry.CountryId} was not found when creating hotel data"));
        }

        mapper.Map(updateEntry, hotel);
        context.Hotels.Update(hotel);
        await context.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        var countryExists = await countryService.CountryExistsAsync(hotelDto.CountryId);
        if (!countryExists)
        {
            return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Country with id {hotelDto.CountryId} was not found while creating Hotel data."));
        }
        var duplicate = await HotelExistsAsync(hotelDto.Name, hotelDto.CountryId);
        if (duplicate)
        {
            return Result<GetHotelDto>.Failure(new Error(ErrorCodes.Conflict, $"Hotel with name {hotelDto.Name} already exists in selected country"));
        }

        var hotel = mapper.Map<Hotel>(hotelDto);
        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();

        var dto = await context.Hotels
            .Where(h => h.Id == hotel.Id)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .FirstAsync();

        return Result<GetHotelDto>.Success(dto);
    }

    public async Task<Result> DeleteHotelAsync(int id)
    {
        var affected = await context.Hotels
            .Where(h => h.Id == id)
            .ExecuteDeleteAsync();
        if(affected == 0)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"No hotel with id {id} was found. Nothing was deleted"));
        }
        return Result.Success();
    }

    public async Task<bool> HotelExistsAsync(int id)
    {
        return await context.Hotels.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> HotelExistsAsync(string name, int countryId)
    {
        return await context.Hotels
            .AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim() && e.CountryId == countryId);
    }

}
