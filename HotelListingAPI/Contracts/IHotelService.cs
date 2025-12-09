using HotelListingAPI.DTO.Hotel;
using HotelListingAPI.Results;

namespace HotelListingAPI.Contracts
{
    public interface IHotelService
    {
        Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto);
        Task<Result> DeleteHotelAsync(int id);
        Task<Result<GetHotelDto>> GetHotelAsync(int id);
        Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync();
        Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateEntry);

        Task<bool> HotelExistsAsync(int id);
        Task<bool> HotelExistsAsync(string name, int countryId);



    }
}