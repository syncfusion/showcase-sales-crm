using SalesCrm.Domain.Entities;

namespace SalesCrm.SeedData;

public sealed class TerritoryBogusGenerator
{
    public IReadOnlyList<Territory> Generate(IReadOnlyList<User> users, int count)
    {
        var names = new[]
        {
            "Pacific Northwest", "Northern California", "Southern California", "Mountain West",
            "Texas Triangle", "Great Lakes", "New England", "Mid Atlantic",
            "Southeast", "Florida", "Central Canada", "Western Canada"
        };

        var codes = new[] { "PNW", "NCA", "SCA", "MTW", "TEX", "GLK", "NEN", "MAT", "SEA", "FLA", "CCA", "WCA" };
        var coverage = new[] { "Named Accounts", "Commercial", "Enterprise", "Strategic" };

        var territories = new List<Territory>(count);
        for (var i = 0; i < count; i++)
        {
            territories.Add(new Territory(
                DeterministicIds.Territories[i],
                names[i % names.Length],
                codes[i % codes.Length],
                users[(i + 1) % users.Count].Id,
                coverage[i % coverage.Length]));
        }

        return territories;
    }
}
