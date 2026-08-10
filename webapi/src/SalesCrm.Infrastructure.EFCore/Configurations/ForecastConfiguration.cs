using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesCrm.Domain.Entities;

namespace SalesCrm.Infrastructure.EFCore.Configurations;

public sealed class ForecastConfiguration : IEntityTypeConfiguration<Forecast>
{
    public void Configure(EntityTypeBuilder<Forecast> builder)
    {
        builder.ToTable("forecasts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.PeriodKey)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.OwnsOne(x => x.BestCaseAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("best_case_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("best_case_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(x => x.PipelineAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("pipeline_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("pipeline_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(x => x.CommittedAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("commit_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("commit_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(x => x.ForecastCategory).IsRequired().HasMaxLength(100);

        builder.Navigation(x => x.BestCaseAmount).IsRequired();
        builder.Navigation(x => x.PipelineAmount).IsRequired();
    }
}
