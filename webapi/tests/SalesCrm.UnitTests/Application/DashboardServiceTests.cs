using SalesCrm.Application.Services;
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;
using SalesCrm.Domain.ValueObjects;
using SalesCrm.UnitTests.TestDoubles;
using Xunit;

namespace SalesCrm.UnitTests.Application;

public sealed class DashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_ShouldCalculatePipelineForecastAndWinRate()
    {
        var accountId = Guid.NewGuid();
        var account = new Account(accountId, "Acme Cloud Systems", "SaaS", Guid.NewGuid(), Guid.NewGuid());
        var opportunities = new[]
        {
            CreateOpportunity(accountId, OpportunityStage.ClosedWon, 100_000m, 100, new DateTime(2026, 7, 1)),
            CreateOpportunity(accountId, OpportunityStage.ClosedLost, 50_000m, 0, new DateTime(2026, 7, 8)),
            CreateOpportunity(accountId, OpportunityStage.Negotiation, 200_000m, 50, new DateTime(2026, 8, 1))
        };
        var forecasts = new[]
        {
            new Forecast(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Commit", new Money(75_000m, "USD"), new Money(90_000m, "USD"), new Money(100_000m, "USD"), "2026-07")
        };
        var service = new DashboardService(
            new InMemoryReadRepository<Opportunity, Guid>(opportunities, item => item.Id),
            new InMemoryReadRepository<Account, Guid>([account], item => item.Id),
            new InMemoryReadRepository<Forecast, Guid>(forecasts, item => item.Id));

        var result = await service.GetDashboardAsync(TestContext.Current.CancellationToken);

        Assert.Equal(75_000m, result.Summary.Revenue);
        Assert.Equal(350_000m, result.Summary.PipelineValue);
        Assert.Equal(200_000m, result.Summary.WeightedForecast);
        Assert.Equal(50m, result.Summary.WinRate);
        Assert.Equal(3, result.Summary.RecentOpportunities.Count);
        Assert.Contains(result.Summary.StageBreakdown, item => item.Stage == OpportunityStage.Negotiation.ToString() && item.Value == 200_000m);
    }

    private static Opportunity CreateOpportunity(Guid accountId, OpportunityStage stage, decimal amount, int probability, DateTime closeDate)
        => new(Guid.NewGuid(), accountId, null, Guid.NewGuid(), stage, "Pipeline", new Money(amount, "USD"), probability, DateTime.SpecifyKind(closeDate, DateTimeKind.Utc));
}
