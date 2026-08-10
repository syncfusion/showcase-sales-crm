using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;

namespace SalesCrm.SeedData;

public sealed class ContactBogusGenerator
{
    public IReadOnlyList<Contact> Generate(int seed, IReadOnlyList<Account> accounts, int count)
    {
        var rng = new Random(seed);
        var contactNames = new (string FirstName, string LastName)[]
        {
            ("Ava", "Carter"), ("Caleb", "Nguyen"), ("Maya", "Patel"), ("Ethan", "Bennett"),
            ("Sophia", "Martinez"), ("Noah", "Williams"), ("Aisha", "Johnson"), ("Daniel", "Chen"),
            ("Priya", "Raman"), ("Marcus", "Lee"), ("Elena", "Garcia"), ("Jordan", "Miller"),
            ("Nina", "Shah"), ("Omar", "Haddad"), ("Ivy", "Kim"), ("Lucas", "Reed"),
            ("Sara", "Lopez"), ("Mason", "Brooks"), ("Grace", "Turner"), ("Leo", "Foster"),
            ("Mia", "Wilson"), ("Rafael", "Santos"), ("Zoe", "Anderson"), ("Tyler", "Morgan"),
            ("Amara", "Okafor"), ("Liam", "O'Connor"), ("Hannah", "Stone"), ("Evan", "Parker"),
            ("Leah", "Goldberg"), ("Adrian", "Rivera"), ("Naomi", "Clark"), ("Julian", "Ross"),
            ("Chloe", "Evans"), ("Isaac", "Ward"), ("Sofia", "Moretti"), ("Miles", "Cooper"),
            ("Tara", "Desai"), ("Ben", "Collins"), ("Mei", "Lin"), ("Victor", "Hughes"),
            ("Layla", "Mansour"), ("Henry", "Price"), ("Anika", "Verma"), ("Cole", "Richardson"),
            ("Elise", "Watson"), ("Rohan", "Kapoor"), ("Mila", "Hart"), ("Theo", "James"),
            ("Jasmine", "Ahmed"), ("Kai", "Tanaka"), ("Riley", "Murphy"), ("Diana", "Scott"),
            ("Aria", "Bose"), ("Mateo", "Silva"), ("Sienna", "Grant"), ("Elliot", "Hayes"),
            ("Lena", "Fischer"), ("Nolan", "Wright"), ("Farah", "Khan"), ("Cameron", "Bell"),
            ("Emily", "Davis"), ("Nathan", "Gray"), ("Olivia", "Moore"), ("Simon", "Bailey")
        };
        var roles = new[] { "CFO", "VP Sales", "Head of Revenue Operations", "Procurement Lead", "Security Architect", "Implementation Director" };

        var items = new List<Contact>(count);
        for (var i = 0; i < count; i++)
        {
            var account = accounts[i % accounts.Count];
            var contactName = contactNames[i % contactNames.Length];
            var domain = string.Concat(account.Name
                .Where(char.IsLetterOrDigit))
                .ToLowerInvariant();
            var role = new[]
            {
                ContactRoleType.DecisionMaker,
                ContactRoleType.Influencer,
                ContactRoleType.Champion,
                ContactRoleType.TechnicalEvaluator,
                ContactRoleType.Other
            }[i % 5];
            items.Add(new Contact(
                DeterministicIds.Contacts[i],
                account.Id,
                contactName.FirstName,
                contactName.LastName,
                $"{contactName.FirstName.ToLowerInvariant()}.{contactName.LastName.Replace("'", string.Empty).ToLowerInvariant()}@{domain}.example",
                role));
            items[^1].UpdateJobTitle(roles[i % roles.Length]);
            items[^1].UpdatePhone($"+1-555-{rng.Next(100, 999)}-{rng.Next(1000, 9999)}");
        }

        return items;
    }
}
