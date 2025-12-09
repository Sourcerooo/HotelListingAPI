using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.DTO.Country;
using HotelListingAPI.DTO.Hotel;

namespace HotelListingAPI.MappingProfiles;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, GetHotelDto>()
            .ForMember(d => d.Country, cfg => cfg.MapFrom<CountryNameResolver>());
        CreateMap<Hotel, GetHotelSlimDto>();
        CreateMap<CreateHotelDto, Hotel>();
    }
}


public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>()
            .ForMember(d=>d.Id, opt=>opt.MapFrom(s=>s.CountryId));
        CreateMap<Country, GetCountriesDto>()
            .ForMember(d=>d.Id, opt=>opt.MapFrom(s=>s.CountryId));

        CreateMap<CreateCountryDto, Country>();
    }
}

public class CountryNameResolver : IValueResolver<Hotel, GetHotelDto, string>
{
    public string Resolve(Hotel source, GetHotelDto destination, string destMember, ResolutionContext context)
    {
        return source.Country?.Name ?? string.Empty;
    }
}
