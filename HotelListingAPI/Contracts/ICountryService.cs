using HotelListingAPI.DTO.Country;
using HotelListingAPI.Results;

namespace HotelListingAPI.Contracts;

public interface ICountryService
{
    Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto country);
    Task<Result> DeleteCountryAsync(int id);
    Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result> UpdateCountryAsync(int Id, UpdateCountryDto country);

    Task<bool> CountryExistsAsync(int id);
    Task<bool> CountryExistsAsync(string name);
}