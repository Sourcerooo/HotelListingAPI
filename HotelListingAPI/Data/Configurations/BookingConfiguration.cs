using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.HasIndex(b => b.HotelId);
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => new { b.CheckInDate, b.CheckOutDate });
    }
}
