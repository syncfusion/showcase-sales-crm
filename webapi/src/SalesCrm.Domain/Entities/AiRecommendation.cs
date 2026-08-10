using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class AiRecommendation : AuditableEntity<Guid>
{
    public AiRecommendation(Guid id, string targetType, Guid targetId, string suggestedAction, string reason, string supportingEvidence, string priority)
        : base(id)
    {
        TargetType = RequireText(targetType, nameof(targetType));
        TargetId = targetId;
        SuggestedAction = RequireText(suggestedAction, nameof(suggestedAction));
        Reason = RequireText(reason, nameof(reason));
        SupportingEvidence = RequireText(supportingEvidence, nameof(supportingEvidence));
        Priority = RequireText(priority, nameof(priority));
    }

    public string TargetType { get; private set; }

    public Guid TargetId { get; private set; }

    public string SuggestedAction { get; private set; }

    public string Reason { get; private set; }

    public string SupportingEvidence { get; private set; }

    public string Priority { get; private set; }

    public DateTime GeneratedOnUtc { get; private set; }

    public string? RiskRuleKey { get; private set; }

    public void MarkGenerated(DateTime generatedOnUtc) => GeneratedOnUtc = generatedOnUtc;

    public void UpdateRiskRule(string? riskRuleKey) => RiskRuleKey = riskRuleKey;

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
