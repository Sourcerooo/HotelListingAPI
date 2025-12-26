using HotelListingAPI.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HotelListingAPI.Data;

public class HotelListingDbContext : IdentityDbContext<ApplicationUser>
{
    public HotelListingDbContext(DbContextOptions<HotelListingDbContext> contextOptions)
        : base(contextOptions) { }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<HotelAdmin> HotelAdmins { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApiKey>(b =>
        {
            b.HasIndex(k => k.Key).IsUnique();
        });
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
