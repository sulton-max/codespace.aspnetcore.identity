using IdentityAuth.Models.Constants;
using IdentityAuth.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityAuth.Dal.EntityConfigurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(new Role
        {
            Id = 1,
            Name = Roles.Customer.ToString(),
            NormalizedName = Roles.Customer.ToString().ToUpper(),
        }, new Role
        {
            Id = 2,
            Name = Roles.Admin.ToString(),
            NormalizedName = Roles.Admin.ToString().ToUpper(),
        });
    }
}