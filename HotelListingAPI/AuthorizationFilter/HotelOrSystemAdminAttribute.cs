using HotelListingAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelListingAPI.AuthorizationFilter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class HotelOrSystemAdminAttribute : TypeFilterAttribute
{
    public HotelOrSystemAdminAttribute() : base(typeof(HotelOrSystemAdminFilter))
    {
    }
}

public class HotelOrSystemAdminFilter(HotelListingDbContext dbContext) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        ClaimsPrincipal? user = context.HttpContext.User;
        if (user is null || user?.Identity?.IsAuthenticated == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var isSystemAdminRole = user!.IsInRole("Administrator");
        if (isSystemAdminRole) {
            return;
        }
        var isHotelAdminRole = user!.IsInRole("Hotel Admin");
        if (!isHotelAdminRole)
        {
            context.Result = new ForbidResult();
            return;
        }

        var userId = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? string.Empty;
        if(string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var hotelIdString = context.RouteData.Values["hotelId"]?.ToString();
        int.TryParse(hotelIdString, out var hotelId);
        var isHotelAdmin = await dbContext.HotelAdmins
            .AnyAsync(c => c.UserId == userId && c.HotelId == hotelId);
        if (!isHotelAdmin)
        {
            context.Result = new ForbidResult();
            return;
        }
        return;
    }
}
