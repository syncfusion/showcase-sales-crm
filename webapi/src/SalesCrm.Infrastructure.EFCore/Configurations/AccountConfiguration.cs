using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesCrm.Domain.Entities;

namespace SalesCrm.Infrastructure.EFCore.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Industry).IsRequired().HasMaxLength(150);
        builder.Property(x => x.AnnualRevenueBand).HasMaxLength(100);
        builder.Property(x => x.EmployeeBand).HasMaxLength(100);
        builder.Property(x => x.PrimaryRegion).HasMaxLength(100);
    }
}
