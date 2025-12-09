using HotelListingAPI.Constants;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.DTO.Auth;
using HotelListingAPI.Results;
using Microsoft.AspNetCore.Identity;

namespace HotelListingAPI.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email
        };
        var result = await userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<RegisteredUserDto>.BadRequest(errors);
        }

        return Result<RegisteredUserDto>.Success(new RegisteredUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id
        });
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto userDto)
    {
        var user = await userManager.FindByEmailAsync(userDto.Email);
        if (user is null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, userDto.Password);
        if (!isPasswordValid)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));
        }
        return Result<string>.Success("Login Successful");
    }
}
