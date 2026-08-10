using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;

namespace SalesCrm.SeedData;

public sealed class AccountBogusGenerator
{
    public IReadOnlyList<Account> Generate(int seed, IReadOnlyList<Territory> territories, IReadOnlyList<User> users, int count)
    {
        var rng = new Random(seed);
        var accountNames = new[]
        {
            "Acme Cloud Systems Northwest", "Nimbus Software Group", "Pacific Edge Analytics", "Evergreen Manufacturing",
            "Summit Professional Services", "BluePeak Logistics", "Northstar Industrial Solutions", "Vertex Cloud Technologies",
            "Adventure Works Manufacturing", "Adventure Works Consulting", "Pioneer BioTech Labs", "NovaPay Financial",
            "Apex Legal Group", "Fabrikam Capital Partners", "Atlas Security Systems", "Cedar HR Solutions",
            "UrbanGrid Energy", "Contoso Health Network", "BrightPath Advisory", "Northwind Digital Commerce",
            "Litware Revenue Analytics", "Alpine Supply Chain", "Clearwater SaaS Holdings", "Silverline Industrial",
            "Redwood Customer Operations", "HarborPoint Professional Services", "Keystone Cloud Platforms", "Crescent Data Works",
            "Ironwood Manufacturing Group", "Meridian Field Services", "Skyline Retail Systems", "Riverstone Compliance",
            "Westbridge Analytics", "Oakmont Healthcare", "BlueRiver Payment Systems", "Cobalt Automation",
            "SummitWorks Enterprise", "Vertex Industrial Cloud", "PacificEdge Services", "Greenfield Logistics",
            "Northstar Consulting Group", "Evergreen Robotics", "Aster Legal Operations", "Nimbus Enterprise Cloud",
            "Acme Cloud Systems Texas", "Acme Cloud Systems Enterprise", "Adventure Works Services", "BluePeak Distribution"
        };
        var industries = new[] { "SaaS", "Healthcare", "Manufacturing", "Financial Services", "Professional Services", "Retail" };
        var statuses = new[]
        {
            AccountStatus.Prospect,
            AccountStatus.Active,
            AccountStatus.AtRisk,
            AccountStatus.Inactive
        };

        var items = new List<Account>(count);
        for (var i = 0; i < count; i++)
        {
            var id = DeterministicIds.Accounts[i];
            var account = new Account(
                id,
                accountNames[i % accountNames.Length],
                industries[i % industries.Length],
                territories[i % territories.Count].Id,
                users[(i + 3) % users.Count].Id);
            account.UpdateStatus(statuses[i % statuses.Length]);
            account.UpdateSegments(
                $"${2 + rng.Next(1, 80)}M-${5 + rng.Next(8, 180)}M",
                $"{100 + (i * 11)}-{250 + (i * 18)}",
                territories[i % territories.Count].Name);
            account.TouchActivity(DateTime.SpecifyKind(new DateTime(2026, 6, 23).AddDays(-(i % 21)), DateTimeKind.Utc));
            items.Add(account);
        }

        return items;
    }
}
