using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;
using SalesCrm.Domain.ValueObjects;

namespace SalesCrm.SeedData;

public sealed class OpportunityBogusGenerator
{
    public IReadOnlyList<Opportunity> Generate(IReadOnlyList<Account> accounts, IReadOnlyList<Contact> contacts, IReadOnlyList<User> users, int count)
    {
        if (accounts.Count == 0)
        {
            throw new InvalidOperationException("Cannot generate opportunities without accounts.");
        }

        if (contacts.Count == 0)
        {
            throw new InvalidOperationException("Cannot generate opportunities without contacts.");
        }

        if (count > DeterministicIds.Opportunities.Length)
        {
            throw new InvalidOperationException($"Opportunity count {count} exceeds deterministic ID count {DeterministicIds.Opportunities.Length}.");
        }

        var stages = new[]
        {
            OpportunityStage.Qualification,
            OpportunityStage.Discovery,
            OpportunityStage.Proposal,
            OpportunityStage.Negotiation,
            OpportunityStage.ClosedWon,
            OpportunityStage.ClosedLost
        };
        var forecastCategories = new[] { "Pipeline", "Best Case", "Commit", "Omitted", "Closed" };
        var riskLevels = new[] { "Low", "Medium", "High", "High", "Medium", "Low" };

        // Stable, stage-aware expected close date so the dashboard and opportunities grids read
        // like a real CRM: closed deals (Won/Lost) close in the past (today or earlier), while
        // active-stage deals close in the future (after today). Each opportunity uses a
        // deterministic offset so re-seeding produces the same dates within a day.
        var today = DateTime.Today;

        var opportunities = new List<Opportunity>(count);

        for (var i = 0; i < count; i++)
        {
            var account = accounts[i % accounts.Count];
            var contact = contacts[i % contacts.Count];
            var amount = new Money(45_000m + (i % 20 * 12_500m) + (i / 20 * 80_000m), "USD");
            var stage = stages[i % stages.Length];
            var closeDate = BuildExpectedCloseDate(today, stage, i);

            var opportunity = new Opportunity(
                DeterministicIds.Opportunities[i],
                account.Id,
                contact.Id,
                users[i % users.Count].Id,
                stage,
                forecastCategories[i % forecastCategories.Length],
                amount,
                Math.Min(95, 25 + (i % 12 * 6)),
                DateTime.SpecifyKind(closeDate, DateTimeKind.Utc));

            opportunity.UpdateNextAction(new[]
            {
                "Book executive alignment call",
                "Send implementation timeline",
                "Confirm security review owner",
                "Review commercial terms",
                "Prepare mutual action plan"
            }[i % 5]);
            opportunity.UpdateRiskLevel(riskLevels[i % riskLevels.Length]);
            opportunities.Add(opportunity);
        }

        return opportunities;
    }

    /// <summary>
    /// Derives a CRM-realistic expected close date for a seeded opportunity:
    /// closed deals (Won/Lost) close today or earlier; active deals close after today.
    /// The offsets are deterministic in <paramref name="index"/> so each opportunity
    /// reproducibly gets the same date within a single day.
    /// </summary>
    private static DateTime BuildExpectedCloseDate(DateTime today, OpportunityStage stage, int index)
    {
        // Spread past closes across the last ~6 months; the earliest offset is 1 day ago so a
        // closed deal never reports a future close date.
        const int pastSpanDays = 180;
        // Spread active closes across the next ~5 months.
        const int futureSpanDays = 150;

        return stage switch
        {
            OpportunityStage.ClosedWon => today.AddDays(-((index % pastSpanDays) + 1)),
            OpportunityStage.ClosedLost => today.AddDays(-(((index + 7) % pastSpanDays) + 1)),
            _ => today.AddDays(((index % futureSpanDays) + 7))
        };
    }
}
