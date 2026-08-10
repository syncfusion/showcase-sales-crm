using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.ValueObjects;

namespace SalesCrm.Infrastructure.EFCore.Configurations;

public sealed class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        builder.ToTable("opportunities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ForecastCategory).IsRequired().HasMaxLength(100);
        builder.Property(x => x.RiskLevel).IsRequired().HasMaxLength(50);
        builder.Property(x => x.NextAction).HasMaxLength(250);
        builder.OwnsOne(x => x.EstimatedValue, owned =>
        {
            owned.Property(x => x.Amount).HasColumnName("estimated_value_amount");
            owned.Property(x => x.Currency).HasColumnName("estimated_value_currency").HasMaxLength(10);
        });
    }
}
