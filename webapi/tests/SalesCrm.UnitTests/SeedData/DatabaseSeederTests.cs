using System.Text.RegularExpressions;
using SalesCrm.SeedData;
using Xunit;

namespace SalesCrm.UnitTests.SeedData;

public sealed class DatabaseSeederTests
{
    [Fact]
    public void Generate_ShouldCreateProfessionalDemoDataWithoutNumberedAccountNames()
    {
        var seedData = new DatabaseSeeder(new CrmSeedOptions()).Generate();

        Assert.Equal(48, seedData.Accounts.Count);
        Assert.Equal(64, seedData.Contacts.Count);
        Assert.Equal(60, seedData.Opportunities.Count);
        Assert.DoesNotContain(seedData.Accounts, account => Regex.IsMatch(account.Name, @"\s\d+$"));
        Assert.Equal(seedData.Contacts.Count, seedData.Contacts.Select(contact => contact.FullName).Distinct().Count());
        Assert.All(seedData.Opportunities, opportunity => Assert.InRange(opportunity.ProbabilityPercent, 0, 100));
    }
}
