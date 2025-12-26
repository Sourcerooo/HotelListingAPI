using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id="8d04dce2-969a-435d-bba4-df3f325983dc",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "3b668ac5-46a2-442e-9578-4d9a4ba54aa1"

            },
            new IdentityRole
            {
                Id="f4b2c5e4-8c3f-4b2e-9c5e-4b2c5e4b2c5",
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "d74103ec-999e-42e4-9bd8-95351219d330"
            },
            new IdentityRole
            {
                Id = "f4b2c5e4-8c3f-4b2e-9c5e-4b2c5e4b2c6",
                Name = "Hotel Admin",
                NormalizedName = "HOTEL ADMIN",
                ConcurrencyStamp = "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
            }
        );
    }
}
