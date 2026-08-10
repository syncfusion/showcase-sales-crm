using SalesCrm.Application.Services;
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;
using SalesCrm.Domain.ValueObjects;
using SalesCrm.UnitTests.TestDoubles;
using Xunit;

namespace SalesCrm.UnitTests.Application;

public sealed class TerritoryServiceTests
{
    [Fact]
    public async Task GetTerritoriesAsync_ShouldExcludeLostRevenueAndFallbackToActivePipelineProbability()
    {
        var territory = new Territory(Guid.NewGuid(), "Great Lakes", "GLK", Guid.NewGuid(), "Named Accounts");
        var account = new Account(Guid.NewGuid(), "Northstar Industrial Solutions", "Manufacturing", territory.Id, Guid.NewGuid());
        var opportunities = new[]
        {
            CreateOpportunity(account.Id, OpportunityStage.ClosedLost, 250_000m, 0),
            CreateOpportunity(account.Id, OpportunityStage.Negotiation, 100_000m, 60),
            CreateOpportunity(account.Id, OpportunityStage.Proposal, 50_000m, 40)
        };
        var service = new TerritoryService(
            new InMemoryReadRepository<Territory, Guid>([territory], item => item.Id),
            new InMemoryReadRepository<Account, Guid>([account], item => item.Id),
            new InMemoryReadRepository<Opportunity, Guid>(opportunities, item => item.Id));

        var result = await service.GetTerritoriesAsync(TestContext.Current.CancellationToken);

        var summary = Assert.Single(result);
        Assert.Equal(150_000m, summary.Revenue);
        Assert.Equal(1, summary.Accounts);
        Assert.Equal(50, summary.WinRate);
        Assert.Equal(41.8781, summary.Latitude);
    }

    private static Opportunity CreateOpportunity(Guid accountId, OpportunityStage stage, decimal amount, int probability)
        => new(Guid.NewGuid(), accountId, null, Guid.NewGuid(), stage, "Pipeline", new Money(amount, "USD"), probability, DateTime.SpecifyKind(new DateTime(2026, 7, 1), DateTimeKind.Utc));
}
