using HotelListingAPI.Contracts;
using HotelListingAPI.DTO.Country;
using HotelListingAPI.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelListingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountryController(ICountryService countriesService) : BaseApiController
{
    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var result = await countriesService.GetCountriesAsync();
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var result = await countriesService.GetCountryAsync(id);
        return ToActionResult(result);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto country)
    {
        var result = await countriesService.UpdateCountryAsync(id, country);
        return ToActionResult(result);
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<GetCountryDto>> PostCountry(CreateCountryDto country)
    {
        var result = await countriesService.CreateCountryAsync(country);
        if (!result.IsSuccess)
        {
            return MapErrorsToResponse(result.Errors);
        }
        return CreatedAtAction(nameof(GetCountry), new { id = result.Value!.Id }, result.Value);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles ="Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var result = await countriesService.DeleteCountryAsync(id);
        return ToActionResult(result);
    }
}
