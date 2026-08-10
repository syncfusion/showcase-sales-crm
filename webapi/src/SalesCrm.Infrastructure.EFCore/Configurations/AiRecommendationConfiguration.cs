using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesCrm.Domain.Entities;

namespace SalesCrm.Infrastructure.EFCore.Configurations;

public sealed class AiRecommendationConfiguration : IEntityTypeConfiguration<AiRecommendation>
{
    public void Configure(EntityTypeBuilder<AiRecommendation> builder)
    {
        builder.ToTable("ai_recommendations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TargetType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SuggestedAction).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.SupportingEvidence).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Priority).IsRequired().HasMaxLength(50);
        builder.Property(x => x.RiskRuleKey).HasMaxLength(100);
    }
}
