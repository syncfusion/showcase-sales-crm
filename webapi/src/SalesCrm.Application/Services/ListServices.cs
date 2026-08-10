using SalesCrm.Contracts.Accounts;
using SalesCrm.Contracts.Activities;
using SalesCrm.Contracts.Ai;
using SalesCrm.Contracts.Contacts;
using SalesCrm.Contracts.Documents;
using SalesCrm.Contracts.Forecast;
using SalesCrm.Contracts.Opportunities;
using SalesCrm.Contracts.Pipeline;
using SalesCrm.Contracts.Territories;
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;
using SalesCrm.Infrastructure.Abstractions.Repositories;
using System.Globalization;

namespace SalesCrm.Application.Services;

public interface IAccountService
{
    Task<IReadOnlyList<AccountListItemDto>> GetAccountsAsync(CancellationToken cancellationToken = default);
}

public interface IContactService
{
    Task<IReadOnlyList<ContactListItemDto>> GetContactsAsync(CancellationToken cancellationToken = default);
}

public interface IOpportunityService
{
    Task<IReadOnlyList<OpportunityListItemDto>> GetOpportunitiesAsync(CancellationToken cancellationToken = default);

    Task UpdateStageAsync(Guid opportunityId, string stage, CancellationToken cancellationToken = default);
}

public interface IPipelineService
{
    Task<IReadOnlyList<PipelineCardDto>> GetStagesAsync(CancellationToken cancellationToken = default);
}

public interface IForecastService
{
    Task<ForecastSummaryDto> GetForecastAsync(CancellationToken cancellationToken = default);
}

public interface IActivityService
{
    Task<IReadOnlyList<ActivityListItemDto>> GetActivitiesAsync(CancellationToken cancellationToken = default);
}

public interface ITerritoryService
{
    Task<IReadOnlyList<TerritorySummaryDto>> GetTerritoriesAsync(CancellationToken cancellationToken = default);
}

public interface IDocumentService
{
    Task<IReadOnlyList<DocumentTemplateDto>> GetDocumentsAsync(CancellationToken cancellationToken = default);
}

public interface IAiRecommendationService
{
    Task<IReadOnlyList<AiRecommendationDto>> GetRecommendationsAsync(CancellationToken cancellationToken = default);
}

public sealed class AccountService(
    IReadRepository<Account, Guid> repository,
    IReadRepository<Opportunity, Guid> opportunityRepository,
    IReadRepository<User, Guid> userRepository) : IAccountService
{
    public async Task<IReadOnlyList<AccountListItemDto>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        var accounts = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderBy(x => x.Name).ToList();
        var opportunities = await opportunityRepository.ListAsync(cancellationToken: cancellationToken);
        var users = (await userRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.DisplayName);
        return accounts.Select(account => new AccountListItemDto(
            account.Id.ToString(),
            account.Name,
            account.Industry,
            account.PrimaryRegion ?? string.Empty,
            account.Status.ToString(),
            users.GetValueOrDefault(account.OwnerUserId, "Unassigned"),
            opportunities.Where(x => x.AccountId == account.Id).Sum(x => x.EstimatedValue.Amount),
            CalculateHealthScore(account, opportunities.Count(x => x.AccountId == account.Id && x.RiskLevel == "High")))).ToList();
    }

    private static int CalculateHealthScore(Account account, int highRiskOpportunities)
        => Math.Clamp(account.Status switch
        {
            Domain.Enums.AccountStatus.Active => 84,
            Domain.Enums.AccountStatus.Prospect => 68,
            Domain.Enums.AccountStatus.AtRisk => 42,
            _ => 55
        } - (highRiskOpportunities * 8), 15, 96);
}

public sealed class ContactService(IReadRepository<Contact, Guid> repository, IReadRepository<Account, Guid> accountRepository) : IContactService
{
    public async Task<IReadOnlyList<ContactListItemDto>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        var contacts = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderBy(x => x.FullName).ToList();
        var accounts = (await accountRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.Name);
        return contacts.Select(contact => new ContactListItemDto(
            contact.Id.ToString(),
            contact.FullName,
            accounts.GetValueOrDefault(contact.AccountId, "Unknown Account"),
            contact.JobTitle ?? string.Empty,
            contact.Email)).ToList();
    }
}

public sealed class OpportunityService(
    IReadRepository<Opportunity, Guid> repository,
    IRepository<Opportunity, Guid> writeRepository,
    IReadRepository<Account, Guid> accountRepository,
    IReadRepository<User, Guid> userRepository,
    IUnitOfWork unitOfWork) : IOpportunityService
{
    public async Task<IReadOnlyList<OpportunityListItemDto>> GetOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        var opportunities = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderByDescending(x => x.ExpectedCloseDate).ToList();
        var accounts = (await accountRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.Name);
        var users = (await userRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.DisplayName);
        return opportunities.Select(opportunity => new OpportunityListItemDto(
            opportunity.Id.ToString(),
            BuildOpportunityName(opportunity),
            accounts.GetValueOrDefault(opportunity.AccountId, "Unknown Account"),
            StageLabel(opportunity.Stage),
            opportunity.EstimatedValue.Amount,
            opportunity.ProbabilityPercent,
            DateOnly.FromDateTime(opportunity.ExpectedCloseDate),
            users.GetValueOrDefault(opportunity.OwnerUserId, "Unassigned"))).ToList();
    }

    public async Task UpdateStageAsync(Guid opportunityId, string stage, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(stage))
        {
            throw new ArgumentException("Stage must be provided.", nameof(stage));
        }

        if (!Enum.TryParse<OpportunityStage>(stage, ignoreCase: true, out var parsedStage))
        {
            throw new ArgumentException($"Unknown opportunity stage '{stage}'.", nameof(stage));
        }

        var opportunity = await writeRepository.GetByIdAsync(opportunityId, cancellationToken)
            ?? throw new KeyNotFoundException($"Opportunity '{opportunityId}' was not found.");

        opportunity.MoveToStage(parsedStage);
        writeRepository.Update(opportunity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Derives a realistic, stage-appropriate opportunity name so the Opportunities grid
    /// shows deal-activity names instead of fabricated "Account - ForecastCategory" text.</summary>
    private static string BuildOpportunityName(Opportunity opportunity)
        => opportunity.NextAction switch
        {
            "Book executive alignment call" => "Executive Alignment",
            "Send implementation timeline" => "Implementation Timeline Review",
            "Confirm security review owner" => "Security Review",
            "Review commercial terms" => "Commercial Terms Review",
            "Prepare mutual action plan" => "Mutual Action Plan",
            _ => opportunity.Stage switch
            {
                OpportunityStage.Qualification => "Qualification Review",
                OpportunityStage.Discovery => "Discovery Workshop",
                OpportunityStage.Proposal => "Proposal Review",
                OpportunityStage.Negotiation => "Negotiation Plan",
                OpportunityStage.ClosedWon => "Closed-Won Handoff",
                OpportunityStage.ClosedLost => "Closed-Lost Review",
                _ => "Opportunity Plan"
            }
        };

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
}

public sealed class PipelineService(
    IReadRepository<Opportunity, Guid> repository,
    IReadRepository<Account, Guid> accountRepository,
    IReadRepository<User, Guid> userRepository) : IPipelineService
{
    public async Task<IReadOnlyList<PipelineCardDto>> GetStagesAsync(CancellationToken cancellationToken = default)
    {
        var opportunities = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderBy(x => x.Stage).ThenByDescending(x => x.ProbabilityPercent).ToList();
        var accounts = (await accountRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.Name);
        var users = (await userRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.DisplayName);
        var stageSummaries = opportunities
            .GroupBy(opportunity => opportunity.Stage)
            .ToDictionary(
                group => group.Key,
                group => (Total: group.Sum(opportunity => opportunity.EstimatedValue.Amount), Count: group.Count()));

        return opportunities.Select(opportunity => new PipelineCardDto(
            opportunity.Id.ToString(),
            BuildOpportunityName(opportunity),
            accounts.GetValueOrDefault(opportunity.AccountId, "Unknown Account"),
            opportunity.Stage.ToString(),
            opportunity.EstimatedValue.Amount,
            opportunity.ForecastCategory,
            users.GetValueOrDefault(opportunity.OwnerUserId, "Unassigned"),
            DateOnly.FromDateTime(opportunity.ExpectedCloseDate),
            opportunity.ProbabilityPercent,
            opportunity.RiskLevel,
            stageSummaries.GetValueOrDefault(opportunity.Stage).Total,
            stageSummaries.GetValueOrDefault(opportunity.Stage).Count,
            opportunity.Stage == OpportunityStage.ClosedWon ? DateOnly.FromDateTime(opportunity.ExpectedCloseDate) : null,
            opportunity.Stage == OpportunityStage.ClosedLost ? DateOnly.FromDateTime(opportunity.ExpectedCloseDate) : null,
            BuildLossReason(opportunity),
            CalculateSalesCycleDays(opportunity))).ToList();
    }

    private static string BuildOpportunityName(Opportunity opportunity)
        => opportunity.NextAction switch
        {
            "Book executive alignment call" => "Executive Alignment",
            "Send implementation timeline" => "Implementation Timeline Review",
            "Confirm security review owner" => "Security Review",
            "Review commercial terms" => "Commercial Terms Review",
            "Prepare mutual action plan" => "Mutual Action Plan",
            _ => opportunity.Stage switch
            {
                OpportunityStage.Qualification => "Qualification Review",
                OpportunityStage.Discovery => "Discovery Workshop",
                OpportunityStage.Proposal => "Proposal Review",
                OpportunityStage.Negotiation => "Negotiation Plan",
                OpportunityStage.ClosedWon => "Closed-Won Handoff",
                OpportunityStage.ClosedLost => "Closed-Lost Review",
                _ => "Opportunity Plan"
            }
        };

    private static string? BuildLossReason(Opportunity opportunity)
    {
        if (opportunity.Stage != OpportunityStage.ClosedLost)
        {
            return null;
        }

        return opportunity.RiskLevel switch
        {
            "High" => "Security review stalled",
            _ when opportunity.ProbabilityPercent < 40 => "Budget deferred",
            _ when opportunity.ProbabilityPercent < 60 => "No decision",
            _ => "Timing misalignment"
        };
    }

    private static int? CalculateSalesCycleDays(Opportunity opportunity)
    {
        if (opportunity.Stage != OpportunityStage.ClosedWon)
        {
            return null;
        }

        if (opportunity.CreatedOnUtc == default)
        {
            return 45 + (opportunity.Id.ToByteArray()[0] % 36);
        }

        return Math.Max(1, (opportunity.ExpectedCloseDate.Date - opportunity.CreatedOnUtc.Date).Days);
    }
}

//public sealed class ForecastService(IReadRepository<Forecast, Guid> repository) : IForecastService
//{
//    public async Task<ForecastSummaryDto> GetForecastAsync(CancellationToken cancellationToken = default)
//    {
//        var forecasts = await repository.ListAsync(cancellationToken: cancellationToken);
//        var committed = forecasts.Sum(x => x.CommittedAmount.Amount);
//        var pipeline = forecasts.Sum(x => x.PipelineAmount.Amount);
//        var bestCase = forecasts.Sum(x => x.BestCaseAmount.Amount);
//        var weighted = forecasts.Sum(x => x.PipelineAmount.Amount);
//        var trend = forecasts
//            .GroupBy(x => x.PeriodKey)
//            .OrderBy(x => x.Key)
//            .Select(group =>
//            {
//                var periodCommit = group.Sum(x => x.CommittedAmount.Amount);
//                var periodBestCase = group.Sum(x => x.BestCaseAmount.Amount);
//                var periodPipeline = group.Sum(x => x.PipelineAmount.Amount);
//                var periodWeighted = group.Sum(x => x.PipelineAmount.Amount);
//                return new ForecastTrendPointDto(group.Key, periodCommit, periodBestCase, periodPipeline, periodWeighted);
//            })
//            .ToList();

//        return new ForecastSummaryDto(committed, bestCase, pipeline, weighted, trend);
//    }
//}

//start


public sealed class ForecastService(IReadRepository<Forecast, Guid> repository) : IForecastService
{
    // private const decimal MinimumMonthlyGrowthRate = 0.03m;
    private static readonly decimal[] RealisticMonthlyGrowthRates =
     [
         0.040m,  // Good month
        0.025m,  // Normal growth
       -0.015m,  // Slight dip
        0.055m,  // Strong recovery
       -0.020m,  // Pipeline slippage
        0.045m,  // New deals added
        0.018m,  // Stable growth
       -0.012m,  // Small correction
        0.050m   // Quarter-end push
     ];

    public async Task<ForecastSummaryDto> GetForecastAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await repository.ListAsync(cancellationToken: cancellationToken);

        var rawTrend = forecasts
            .GroupBy(x => x.PeriodKey)
            .OrderBy(x => ParsePeriodKey(x.Key))
            .Select(group =>
            {
                var periodCommit = group.Sum(x => x.CommittedAmount.Amount);
                var periodBestCase = group.Sum(x => x.BestCaseAmount.Amount);
                var periodPipeline = group.Sum(x => x.PipelineAmount.Amount);

                var periodWeighted = CalculateWeightedForecast(
                    periodCommit,
                    periodBestCase,
                    periodPipeline);

                return new MonthlyForecastPoint(
                    group.Key,
                    periodCommit,
                    periodBestCase,
                    periodPipeline,
                    periodWeighted);
            })
            .ToList();

        //var adjustedTrend = ApplyGrowingTrend(rawTrend);
        var adjustedTrend = ApplyRealisticTrend(rawTrend);

        var committed = adjustedTrend.Sum(x => x.CommittedAmount);
        var bestCase = adjustedTrend.Sum(x => x.BestCaseAmount);
        var pipeline = adjustedTrend.Sum(x => x.PipelineAmount);
        var weighted = adjustedTrend.Sum(x => x.WeightedAmount);

        var trend = adjustedTrend
            .Select(x => new ForecastTrendPointDto(
                x.PeriodKey,
                x.CommittedAmount,
                x.BestCaseAmount,
                x.PipelineAmount,
                x.WeightedAmount))
            .ToList();

        return new ForecastSummaryDto(
            committed,
            bestCase,
            pipeline,
            weighted,
            trend);
    }

    private static decimal CalculateWeightedForecast(
        decimal committedAmount,
        decimal bestCaseAmount,
        decimal pipelineAmount)
    {
        var safeCommitted = Math.Max(0, committedAmount);
        var safeBestCase = Math.Max(safeCommitted, bestCaseAmount);
        var safePipeline = Math.Max(safeBestCase, pipelineAmount);

        var bestCaseUpside = safeBestCase - safeCommitted;
        var pipelineUpside = safePipeline - safeBestCase;

        return RoundCurrency(
            safeCommitted +
            bestCaseUpside * 0.70m +
            pipelineUpside * 0.35m);
    }

    //private static IReadOnlyList<MonthlyForecastPoint> ApplyGrowingTrend(
    //    IReadOnlyList<MonthlyForecastPoint> trend)
    //{
    //    var adjustedTrend = new List<MonthlyForecastPoint>(trend.Count);
    //    MonthlyForecastPoint? previous = null;

    //    foreach (var current in trend)
    //    {
    //        if (previous is null)
    //        {
    //            adjustedTrend.Add(current);
    //            previous = current;
    //            continue;
    //        }

    //        var minimumExpectedWeighted =
    //            previous.WeightedAmount * (1 + MinimumMonthlyGrowthRate);

    //        if (current.WeightedAmount >= minimumExpectedWeighted)
    //        {
    //            adjustedTrend.Add(current);
    //            previous = current;
    //            continue;
    //        }

    //        var adjustedPoint = current.WeightedAmount <= 0
    //            ? new MonthlyForecastPoint(
    //                current.PeriodKey,
    //                RoundCurrency(previous.CommittedAmount * (1 + MinimumMonthlyGrowthRate)),
    //                RoundCurrency(previous.BestCaseAmount * (1 + MinimumMonthlyGrowthRate)),
    //                RoundCurrency(previous.PipelineAmount * (1 + MinimumMonthlyGrowthRate)),
    //                RoundCurrency(minimumExpectedWeighted))
    //            : ScaleForecastPoint(current, minimumExpectedWeighted);

    //        adjustedTrend.Add(adjustedPoint);
    //        previous = adjustedPoint;
    //    }

    //    return adjustedTrend;
    //}

    private static IReadOnlyList<MonthlyForecastPoint> ApplyRealisticTrend(
    IReadOnlyList<MonthlyForecastPoint> trend)
    {
        var adjustedTrend = new List<MonthlyForecastPoint>(trend.Count);

        for (var i = 0; i < trend.Count; i++)
        {
            var current = trend[i];

            if (i == 0)
            {
                adjustedTrend.Add(current);
                continue;
            }

            var previous = adjustedTrend[^1];
            var growthRate = GetRealisticMonthlyGrowthRate(i);

            var targetWeightedAmount =
                previous.WeightedAmount * (1 + growthRate);

            var adjustedPoint = ScaleForecastPoint(
                current,
                RoundCurrency(targetWeightedAmount));

            adjustedTrend.Add(adjustedPoint);
        }

        return adjustedTrend;
    }

    private static decimal GetRealisticMonthlyGrowthRate(int monthIndex)
    {
        var patternIndex = (monthIndex - 1) % RealisticMonthlyGrowthRates.Length;
        return RealisticMonthlyGrowthRates[patternIndex];
    }

    private static MonthlyForecastPoint ScaleForecastPoint(
        MonthlyForecastPoint point,
        decimal targetWeightedAmount)
    {
        if (point.WeightedAmount <= 0)
        {
            return new MonthlyForecastPoint(
                point.PeriodKey,
                RoundCurrency(targetWeightedAmount * 0.65m),
                RoundCurrency(targetWeightedAmount * 0.90m),
                RoundCurrency(targetWeightedAmount * 1.20m),
                targetWeightedAmount);
        }

        var scaleFactor = targetWeightedAmount / point.WeightedAmount;

        return new MonthlyForecastPoint(
            point.PeriodKey,
            RoundCurrency(point.CommittedAmount * scaleFactor),
            RoundCurrency(point.BestCaseAmount * scaleFactor),
            RoundCurrency(point.PipelineAmount * scaleFactor),
            targetWeightedAmount);
    }

    //private static MonthlyForecastPoint ScaleForecastPoint(
    //    MonthlyForecastPoint point,
    //    decimal targetWeightedAmount)
    //{
    //    var scaleFactor = targetWeightedAmount / point.WeightedAmount;

    //    return new MonthlyForecastPoint(
    //        point.PeriodKey,
    //        RoundCurrency(point.CommittedAmount * scaleFactor),
    //        RoundCurrency(point.BestCaseAmount * scaleFactor),
    //        RoundCurrency(point.PipelineAmount * scaleFactor),
    //        RoundCurrency(targetWeightedAmount));
    //}

    private static DateOnly ParsePeriodKey(string periodKey)
    {
        return DateOnly.TryParseExact(
            $"{periodKey}-01",
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate)
            ? parsedDate
            : DateOnly.MinValue;
    }

    private static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private sealed record MonthlyForecastPoint(
        string PeriodKey,
        decimal CommittedAmount,
        decimal BestCaseAmount,
        decimal PipelineAmount,
        decimal WeightedAmount);
}
//end

public sealed class ActivityService(
    IReadRepository<Activity, Guid> repository,
    IReadRepository<Account, Guid> accountRepository,
    IReadRepository<Opportunity, Guid> opportunityRepository,
    IReadRepository<User, Guid> userRepository) : IActivityService
{
    public async Task<IReadOnlyList<ActivityListItemDto>> GetActivitiesAsync(CancellationToken cancellationToken = default)
    {
        var activities = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderByDescending(x => x.ScheduledStartUtc).ToList();
        var accounts = (await accountRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.Name);
        var opportunities = (await opportunityRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.ForecastCategory);
        var users = (await userRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.DisplayName);
        return activities.Select(activity => new ActivityListItemDto(
            activity.Id.ToString(),
            $"{activity.ActivityType}: {opportunities.GetValueOrDefault(activity.OpportunityId ?? Guid.Empty, "Account follow-up")}",
            activity.AccountId is { } accountId ? accounts.GetValueOrDefault(accountId, "Unknown Account") : string.Empty,
            activity.ScheduledStartUtc,
            activity.ScheduledEndUtc,
            activity.Status,
            users.GetValueOrDefault(activity.OwnerUserId, "Unassigned"))).ToList();
    }
}

public sealed class TerritoryService(
    IReadRepository<Territory, Guid> repository,
    IReadRepository<Account, Guid> accountRepository,
    IReadRepository<Opportunity, Guid> opportunityRepository) : ITerritoryService
{
    private static readonly Dictionary<string, (double Latitude, double Longitude)> Coordinates = new()
    {
        ["PNW"] = (47.6062, -122.3321),
        ["NCA"] = (37.7749, -122.4194),
        ["SCA"] = (34.0522, -118.2437),
        ["MTW"] = (39.7392, -104.9903),
        ["TEX"] = (32.7767, -96.7970),
        ["GLK"] = (41.8781, -87.6298),
        ["NEN"] = (42.3601, -71.0589),
        ["MAT"] = (40.7128, -74.0060),
        ["SEA"] = (33.7490, -84.3880),
        ["FLA"] = (25.7617, -80.1918),
        ["CCA"] = (43.6532, -79.3832),
        ["WCA"] = (49.2827, -123.1207)
    };

    public async Task<IReadOnlyList<TerritorySummaryDto>> GetTerritoriesAsync(CancellationToken cancellationToken = default)
    {
        var territories = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderBy(x => x.Name).ToList();
        var accounts = await accountRepository.ListAsync(cancellationToken: cancellationToken);
        var opportunities = await opportunityRepository.ListAsync(cancellationToken: cancellationToken);
        return territories.Select(territory =>
        {
            var coordinate = Coordinates.GetValueOrDefault(territory.RegionCode, (39.8283, -98.5795));
            return new TerritorySummaryDto(
                territory.Id.ToString(),
                territory.Name,
                territory.RegionCode,
                opportunities
                    .Where(opportunity => accounts.Any(account => account.Id == opportunity.AccountId && account.TerritoryId == territory.Id)
                        && opportunity.Stage != Domain.Enums.OpportunityStage.ClosedLost)
                    .Sum(x => x.EstimatedValue.Amount),
                accounts.Count(account => account.TerritoryId == territory.Id),
                CalculateWinRate(territory.Id, accounts, opportunities),
                coordinate.Item1,
                coordinate.Item2);
        }).ToList();
    }

    private static int CalculateWinRate(Guid territoryId, IReadOnlyList<Account> accounts, IReadOnlyList<Opportunity> opportunities)
    {
        var accountIds = accounts.Where(account => account.TerritoryId == territoryId).Select(account => account.Id).ToHashSet();
        var territoryOpportunities = opportunities.Where(opportunity => accountIds.Contains(opportunity.AccountId)).ToList();
        var closed = territoryOpportunities.Where(opportunity => opportunity.Stage is Domain.Enums.OpportunityStage.ClosedWon or Domain.Enums.OpportunityStage.ClosedLost).ToList();
        if (closed.Count > 0)
        {
            var closedWinRate = (int)Math.Round(closed.Count(opportunity => opportunity.Stage == Domain.Enums.OpportunityStage.ClosedWon) * 100m / closed.Count);
            if (closedWinRate > 0)
            {
                return closedWinRate;
            }

            var activePipeline = territoryOpportunities.Where(opportunity => opportunity.Stage != Domain.Enums.OpportunityStage.ClosedLost).ToList();
            if (activePipeline.Count > 0)
            {
                return Math.Max(12, (int)Math.Round(activePipeline.Average(opportunity => opportunity.ProbabilityPercent)));
            }

            return 0;
        }

        if (territoryOpportunities.Count == 0)
        {
            return 0;
        }

        return (int)Math.Round(territoryOpportunities.Average(opportunity => opportunity.ProbabilityPercent));
    }
}

public sealed class DocumentService(
    IReadRepository<SalesDocument, Guid> repository,
    IReadRepository<Account, Guid> accountRepository,
    IReadRepository<Opportunity, Guid> opportunityRepository,
    IReadRepository<User, Guid> userRepository) : IDocumentService
{
    public async Task<IReadOnlyList<DocumentTemplateDto>> GetDocumentsAsync(CancellationToken cancellationToken = default)
    {
        var documents = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderBy(x => x.Title).ToList();
        var accounts = (await accountRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.Name);
        var opportunities = (await opportunityRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.ForecastCategory);
        var users = (await userRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.DisplayName);
        return documents.Select(document => new DocumentTemplateDto(
            document.Id.ToString(),
            document.Title,
            document.DocumentType,
            document.ArchivedOnUtc is null ? "Draft" : "Archived",
            users.GetValueOrDefault(document.AuthorUserId, "Unassigned"),
            accounts.GetValueOrDefault(document.AccountId, "Unknown Account"),
            document.OpportunityId is { } opportunityId ? opportunities.GetValueOrDefault(opportunityId, "Opportunity") : "Account-level",
            document.TemplateKey)).ToList();
    }
}

public sealed class AiRecommendationService(
    IReadRepository<AiRecommendation, Guid> repository,
    IReadRepository<Opportunity, Guid> opportunityRepository,
    IReadRepository<Account, Guid> accountRepository) : IAiRecommendationService
{
    public async Task<IReadOnlyList<AiRecommendationDto>> GetRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        var recommendations = (await repository.ListAsync(cancellationToken: cancellationToken)).OrderByDescending(x => x.Priority).ToList();
        var opportunities = (await opportunityRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);
        var accounts = (await accountRepository.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id, x => x.Name);
        return recommendations.Select(recommendation => new AiRecommendationDto(
            recommendation.Id.ToString(),
            recommendation.SuggestedAction,
            recommendation.Reason,
            recommendation.Priority,
            recommendation.SupportingEvidence,
            opportunities.TryGetValue(recommendation.TargetId, out var opportunity)
                ? $"{accounts.GetValueOrDefault(opportunity.AccountId, "Account")} - {opportunity.ForecastCategory}"
                : recommendation.TargetType,
            opportunities.TryGetValue(recommendation.TargetId, out opportunity) ? opportunity.EstimatedValue.Amount : 0,
            opportunities.TryGetValue(recommendation.TargetId, out opportunity) ? opportunity.ExpectedCloseDate.ToString("MMM d, yyyy") : string.Empty)).ToList();
    }
}
