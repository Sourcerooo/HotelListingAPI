using HotelListingAPI.DTO.Auth;
using HotelListingAPI.Results;

namespace HotelListingAPI.Contracts;

public interface IUserService
{
    Task<Result<string>> LoginAsync(LoginUserDto userDto);
    Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
}