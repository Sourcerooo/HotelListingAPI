using HotelListingAPI.Constants;
using HotelListingAPI.Contracts;
using HotelListingAPI.DTO.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace HotelListingAPI.Handlers;

public class ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyValidatorService apiKeyValidatorService
    )
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey = string.Empty;
        if (Request.Headers.TryGetValue(AuthenticationDefaults.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            apiKey = apiKeyHeaderValues.ToString() ?? string.Empty;
        }

        if(string.IsNullOrWhiteSpace(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var valid = await apiKeyValidatorService.IsValidApiKeyAsync(apiKey, Context.RequestAborted);
        if(!valid)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "apikey"),
            new(ClaimTypes.Name, "ApiKeyClient")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
