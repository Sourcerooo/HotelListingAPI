using HotelListingAPI.Constants;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.DTO.Auth;
using HotelListingAPI.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelListingAPI.Services;

public class UserService(UserManager<ApplicationUser> userManager,
    HotelListingDbContext dbContext,
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor) : IUserService
{
    public string UserId
    {
        get
        {
            return httpContextAccessor?
           .HttpContext?
           .User?
           .FindFirst(JwtRegisteredClaimNames.Sub)?
           .Value
           ?? httpContextAccessor ?
           .HttpContext?
           .User?
           .FindFirst(ClaimTypes.NameIdentifier)?
           .Value
           ?? string.Empty;
        }
    }
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

        await userManager.AddToRoleAsync(user, registerUserDto.Role);

        if(registerUserDto.Role == "Hotel Admin")
        {
            var hotelAdmin = dbContext.HotelAdmins.Add(
                new HotelAdmin
                {
                    UserId = user.Id,
                    HotelId = registerUserDto.AssociatedHotelId.GetValueOrDefault()
                });
            await dbContext.SaveChangesAsync();
        }

        return Result<RegisteredUserDto>.Success(new RegisteredUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Id = user.Id,
            Role = registerUserDto.Role
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
        var token = await GenerateJwtTokenAsync(user);
        return Result<string>.Success(token);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        //Basic user claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email??string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, user.FullName)
        };

        //Add roles claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        claims = claims.Union(roleClaims).ToList();

        //Set JWT token settings
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])
            );
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                securityKey,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256
            );

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
