using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingAPI.Constants;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.DTO.Country;
using HotelListingAPI.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace HotelListingAPI.Services;

public class CountryService(HotelListingDbContext context, IMapper mapper) : ICountryService
{
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        var countries = await context.Countries
            .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        var country = await context.Countries
            .Where(c => c.CountryId == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return country is null
             ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."))
            : Result<GetCountryDto>.Success(country);
    }

    public async Task<Result> UpdateCountryAsync(int Id, UpdateCountryDto country)
    {
        try
        {
            if (Id != country.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, $"Parameter Id {Id} does not match Data Id {country.Id}"));
            }

            var entry = await context.Countries.FindAsync(Id);
            if (entry is null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with id {Id} was not found"));
            }


            mapper.Map(country, entry);

            //context.Entry(entry).State = EntityState.Modified;
            //context.Countries.Update(entry);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, "An unexpected error occurred while updating the country."));
        }
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto)
    {
        try
        {
            var exists = await CountryExistsAsync(countryDto.Name);
            if (exists)
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, $"Country with name {countryDto.Name} already exists"));
            }

            var country = mapper.Map<Country>(countryDto);
            context.Countries.Add(country);
            await context.SaveChangesAsync();
            var dto = await context.Countries
             .Where(c => c.CountryId == country.CountryId)
             .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
             .FirstAsync();

            return Result<GetCountryDto>.Success(dto);
        }
        catch (Exception)
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Failure, "An unexpected error occurred while creating the country."));
        }
    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await context.Countries.FindAsync(id);
            if (country == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with id {id} does not exist"));
            }
            context.Countries.Remove(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.CountryId == id);
    }
    public async Task<bool> CountryExistsAsync(string name)
    {
        return await context.Countries.AnyAsync(e => e.Name.Trim().ToLower() == name.Trim().ToLower());
    }
}
