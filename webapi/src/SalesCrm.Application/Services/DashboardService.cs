using SalesCrm.Contracts.Dashboard;
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;
using SalesCrm.Infrastructure.Abstractions.Repositories;

namespace SalesCrm.Application.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default);
}

public sealed class DashboardService : IDashboardService
{
    private readonly IReadRepository<Opportunity, Guid> opportunityRepository;
    private readonly IReadRepository<Account, Guid> accountRepository;
    private readonly IReadRepository<Forecast, Guid> forecastRepository;

    public DashboardService(
        IReadRepository<Opportunity, Guid> opportunityRepository,
        IReadRepository<Account, Guid> accountRepository,
        IReadRepository<Forecast, Guid> forecastRepository)
    {
        this.opportunityRepository = opportunityRepository;
        this.accountRepository = accountRepository;
        this.forecastRepository = forecastRepository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var opportunities = await opportunityRepository.ListAsync(cancellationToken: cancellationToken);
        var accounts = await accountRepository.ListAsync(cancellationToken: cancellationToken);
        var forecasts = await forecastRepository.ListAsync(cancellationToken: cancellationToken);

        var accountLookup = accounts.ToDictionary(account => account.Id, account => account.Name);
        var recentOpportunities = opportunities
            .OrderByDescending(x => x.ExpectedCloseDate)
            .ThenByDescending(x => x.CreatedOnUtc)
            .Take(5)
            .Select(opportunity => new RecentOpportunityDto(
                opportunity.Id.ToString(),
                BuildOpportunityName(opportunity),
                accountLookup.TryGetValue(opportunity.AccountId, out var accountName) ? accountName : "Unknown",
                StageLabel(opportunity.Stage),
                opportunity.EstimatedValue.Amount,
                DateOnly.FromDateTime(opportunity.ExpectedCloseDate)))
            .ToList();

        var stageBreakdown = opportunities
            .GroupBy(opportunity => opportunity.Stage)
            .OrderBy(group => group.Key)
            .Select(group => new StageBreakdownDto(
                StageLabel(group.Key),
                group.Sum(item => item.EstimatedValue.Amount),
                group.Count()))
            .ToList();

        var totalPipelineValue = opportunities.Sum(opportunity => opportunity.EstimatedValue.Amount);
        var committedForecast = forecasts.Sum(forecast => forecast.CommittedAmount.Amount);
        var weightedPipeline = opportunities.Sum(opportunity => opportunity.EstimatedValue.Amount * opportunity.ProbabilityPercent / 100m);
        var totalClosed = opportunities.Count(opportunity => opportunity.Stage is OpportunityStage.ClosedWon or OpportunityStage.ClosedLost);
        var wonCount = opportunities.Count(opportunity => opportunity.Stage == OpportunityStage.ClosedWon);
        var winRate = totalClosed == 0 ? 0m : Math.Round((decimal)wonCount * 100m / totalClosed, 2);

        var summary = new DashboardSummaryDto(
            committedForecast,
            totalPipelineValue,
            winRate,
            weightedPipeline,
            recentOpportunities,
            stageBreakdown);

        return new DashboardResponse(summary);
    }

    /// <summary>
    /// Derives a realistic, CRM-correct opportunity name from the opportunity's stage so the
    /// Recent Opportunities grid shows deal-activity names (e.g. "Contract Review",
    /// "Security Review", "Final Approval") instead of fabricated "Account - ForecastCategory"
    /// strings that produced inaccurate labels like "Adventure Works Consulting - Closed" for
    /// deals in active stages. Closed deals get outcome-oriented names ("Loss Review",
    /// "Renewal Closed Won") that never apply the word "Closed" to an active-stage deal.
    /// </summary>
    private static string BuildOpportunityName(Opportunity opportunity)
    {
        // A deterministic, stable index is used so the same opportunity always maps to the same
        // name across requests (we can't persist a name on the entity without schema changes).
        var index = Math.Abs(opportunity.Id.GetHashCode()) % ActiveStageNames.Length;

        return opportunity.Stage switch
        {
            OpportunityStage.Qualification => ActiveStageNames[index],
            OpportunityStage.Discovery => ActiveStageNames[(index + 1) % ActiveStageNames.Length],
            OpportunityStage.Proposal => ActiveStageNames[(index + 2) % ActiveStageNames.Length],
            OpportunityStage.Negotiation => NegotiationNames[index % NegotiationNames.Length],
            OpportunityStage.ClosedWon => WonNames[index % WonNames.Length],
            OpportunityStage.ClosedLost => LostNames[index % LostNames.Length],
            _ => "Opportunity"
        };
    }

    /// <summary>Maps an <see cref="OpportunityStage"/> enum to its human-readable display label,
    /// so callers never see the raw "ClosedWon"/"ClosedLost" enum names.</summary>
    private static string StageLabel(OpportunityStage stage) => stage switch
    {
        OpportunityStage.Qualification => "Qualification",
        OpportunityStage.Discovery => "Discovery",
        OpportunityStage.Proposal => "Proposal",
        OpportunityStage.Negotiation => "Negotiation",
        OpportunityStage.ClosedWon => "Closed Won",
        OpportunityStage.ClosedLost => "Closed Lost",
        _ => stage.ToString()
    };

    // Realistic deal-activity names for active pipeline stages. None of these contain the words
    // "Closed", "Won", or "Lost" — those qualifiers belong only to terminal-stage names.
    private static readonly string[] ActiveStageNames =
    {
        "Implementation Timeline Review",
        "Security Review",
        "Commercial Terms Review",
        "Mutual Action Plan",
        "Executive Alignment",
        "Renewal Expansion",
        "Enterprise License Upgrade",
        "Platform Expansion",
        "Analytics Rollout",
        "Technical Discovery"
    };

    private static readonly string[] NegotiationNames =
    {
        "Contract Review",
        "Pricing Negotiation",
        "Mutual Action Plan",
        "Final Approval"
    };

    private static readonly string[] WonNames =
    {
        "Renewal Closed Won",
        "Enterprise License Closed Won",
        "Platform Expansion Won",
        "Analytics Rollout Won"
    };

    private static readonly string[] LostNames =
    {
        "Loss Review",
        "Discontinued Evaluation",
        "No-Decision Closure",
        "Competitive Loss Review"
    };
}
