using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListingAPI.Data;

public class ApiKey
{
    public int Id { get; set; }
    [MaxLength(256)]
    public string Key { get; set; } = string.Empty;
    [MaxLength(200)]
    public string AppName { get; set; } = string.Empty;

    public DateTimeOffset? ExpiresOnUtc { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool IsActve => !ExpiresOnUtc.HasValue || ExpiresOnUtc.Value > DateTimeOffset.UtcNow;
}
