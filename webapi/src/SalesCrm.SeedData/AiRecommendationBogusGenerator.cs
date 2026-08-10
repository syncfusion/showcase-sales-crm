using SalesCrm.Domain.Entities;

namespace SalesCrm.SeedData;

public sealed class AiRecommendationBogusGenerator
{
    public IReadOnlyList<AiRecommendation> Generate(IReadOnlyList<Opportunity> opportunities, int count)
    {
        var actions = new[]
        {
            "Confirm economic buyer before next forecast call",
            "Schedule security review with technical evaluator",
            "Send mutual action plan for close-date alignment",
            "Re-engage champion with implementation value summary",
            "Move low-probability renewal out of Commit"
        };

        var items = new List<AiRecommendation>(count);
        for (var i = 0; i < count; i++)
        {
            var opportunity = opportunities[i % opportunities.Count];
            items.Add(new AiRecommendation(
                DeterministicIds.AiRecommendations[i],
                "Opportunity",
                opportunity.Id,
                actions[i % actions.Length],
                $"{opportunity.RiskLevel} risk deal in {opportunity.Stage} with {opportunity.ProbabilityPercent}% probability and {opportunity.ForecastCategory} forecast category.",
                $"Close date {opportunity.ExpectedCloseDate:MMM d}; next action: {opportunity.NextAction}; estimated value {opportunity.EstimatedValue.Amount:C0}.",
                new[] { "High", "Medium", "Low" }[i % 3]));
            items[^1].UpdateRiskRule($"rule-{i % 4 + 1}");
            items[^1].MarkGenerated(DateTime.SpecifyKind(new DateTime(2026, 5, 1).AddDays(i), DateTimeKind.Utc));
        }

        return items;
    }
}
