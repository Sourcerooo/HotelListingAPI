using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.Services;

public class ApiKeyValidatorService(HotelListingDbContext context) : IApiKeyValidatorService
{
    public async Task<bool> IsValidApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        var existingApiKey = await context.ApiKeys
            .AsNoTracking()
            .Where(k => k.Key == apiKey)
            .FirstOrDefaultAsync(ct);
        if(existingApiKey is null)
        {
             return false;
        }
        return existingApiKey.IsActve;
    }
}
