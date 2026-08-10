using SalesCrm.Domain.Entities;

namespace SalesCrm.SeedData;

public sealed class UserBogusGenerator
{
    public IReadOnlyList<User> Generate(int count)
    {
        var names = new[]
        {
            "Maya Raman", "Evan Brooks", "Priya Shah", "Daniel Chen", "Sofia Martinez",
            "Noah Williams", "Iris Nguyen", "Marcus Lee", "Aisha Johnson", "Liam Carter",
            "Nora Patel", "Victor Gomez", "Elena Rossi", "Owen Baker", "Grace Kim"
        };

        var users = new List<User>(count);
        for (var i = 0; i < count; i++)
        {
            var name = names[i % names.Length];
            var email = $"{name.ToLowerInvariant().Replace(" ", ".")}@northstarcrm.example";
            users.Add(new User(DeterministicIds.Users[i], name, email));
        }

        return users;
    }
}
