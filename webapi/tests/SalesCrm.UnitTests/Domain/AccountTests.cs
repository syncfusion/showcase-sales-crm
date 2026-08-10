using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;
using SalesCrm.Domain.ValueObjects;
using Xunit;

namespace SalesCrm.UnitTests.Domain;

public class AccountTests
{
    [Fact]
    public void Constructor_ShouldInitializeAccount()
    {
        var account = new Account(Guid.NewGuid(), "Contoso", "Technology", Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal("Contoso", account.Name);
        Assert.Equal(AccountStatus.Prospect, account.Status);
    }

    [Fact]
    public void Opportunity_ShouldRejectInvalidProbability()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Opportunity(Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), OpportunityStage.Qualification, "Pipeline", new Money(1000m, "USD"), 200, DateTime.UtcNow));

        Assert.Equal("probabilityPercent", ex.ParamName);
    }
}
