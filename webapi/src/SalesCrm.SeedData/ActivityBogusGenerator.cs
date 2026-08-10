using SalesCrm.Domain.Entities;
using SalesCrm.Domain.Enums;

namespace SalesCrm.SeedData;

public sealed class ActivityBogusGenerator
{
    public IReadOnlyList<Activity> Generate(IReadOnlyList<Account> accounts, IReadOnlyList<Contact> contacts, IReadOnlyList<Opportunity> opportunities, IReadOnlyList<User> users, int count)
    {
        if (accounts.Count == 0)
        {
            throw new InvalidOperationException("Cannot generate activities without accounts.");
        }

        if (contacts.Count == 0)
        {
            throw new InvalidOperationException("Cannot generate activities without contacts.");
        }

        if (count > DeterministicIds.Activities.Length)
        {
            throw new InvalidOperationException($"Activity count {count} exceeds deterministic ID count {DeterministicIds.Activities.Length}.");
        }

        var activityTypes = new[] { ActivityType.Call, ActivityType.Email, ActivityType.Meeting, ActivityType.Task, ActivityType.Note };
        var items = new List<Activity>(count);
        var anchor = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

        for (var i = 0; i < count; i++)
        {
            var account = accounts[i % accounts.Count];
            var contact = contacts[i % contacts.Count];
            var opportunity = opportunities[i % opportunities.Count];
            var start = anchor.AddDays(i % 21 - 7).AddHours(i % 8);

            var activity = new Activity(
                DeterministicIds.Activities[i],
                activityTypes[i % activityTypes.Length],
                start,
                start.AddMinutes(i % 3 == 0 ? 45 : 60),
                users[(i + 2) % users.Count].Id);

            activity.LinkToAccount(account.Id);
            activity.LinkToContact(contact.Id);
            activity.LinkToOpportunity(opportunity.Id);
            if (i % 4 == 0)
            {
                activity.Complete($"Completed {activity.ActivityType} with {contact.FullName}; next step captured.");
            }

            items.Add(activity);
        }

        return items;
    }
}
