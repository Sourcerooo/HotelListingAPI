using AutoMapper;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Enums;
using HotelListingAPI.DTO.Booking;
using HotelListingAPI.DTO.Country;
using HotelListingAPI.DTO.Hotel;

namespace HotelListingAPI.MappingProfiles;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, GetHotelDto>()
            .ForMember(d => d.CountryName, cfg => cfg.MapFrom<CountryNameResolver>());
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

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, GetBookingDto>()
            .ForMember(d => d.HotelName, opt => opt.MapFrom(s => s.Hotel!.Name))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateBookingDto, Booking>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.UserId, opt => opt.Ignore())
            .ForMember(d => d.TotalPrice, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.Hotel, opt => opt.Ignore());

        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.UserId, opt => opt.Ignore())
            .ForMember(d => d.TotalPrice, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.Hotel, opt => opt.Ignore());
    }
}
