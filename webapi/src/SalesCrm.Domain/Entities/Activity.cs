using SalesCrm.Domain.Common;
using SalesCrm.Domain.Enums;

namespace SalesCrm.Domain.Entities;

public sealed class Activity : AuditableEntity<Guid>
{
    public Activity(Guid id, ActivityType activityType, DateTime scheduledStartUtc, DateTime scheduledEndUtc, Guid ownerUserId)
        : base(id)
    {
        if (scheduledEndUtc < scheduledStartUtc)
        {
            throw new ArgumentException("End time must not precede start time.");
        }

        ActivityType = activityType;
        ScheduledStartUtc = scheduledStartUtc;
        ScheduledEndUtc = scheduledEndUtc;
        OwnerUserId = ownerUserId;
        Status = "Planned";
    }

    public ActivityType ActivityType { get; private set; }

    public string Status { get; private set; }

    public DateTime ScheduledStartUtc { get; private set; }

    public DateTime ScheduledEndUtc { get; private set; }

    public Guid OwnerUserId { get; private set; }

    public Guid? AccountId { get; private set; }

    public Guid? ContactId { get; private set; }

    public Guid? OpportunityId { get; private set; }

    public string? OutcomeSummary { get; private set; }

    public void LinkToAccount(Guid accountId) => AccountId = accountId;

    public void LinkToContact(Guid contactId) => ContactId = contactId;

    public void LinkToOpportunity(Guid opportunityId) => OpportunityId = opportunityId;

    public void Complete(string? outcomeSummary)
    {
        Status = "Completed";
        OutcomeSummary = outcomeSummary;
    }

    public void Cancel(string? outcomeSummary)
    {
        Status = "Canceled";
        OutcomeSummary = outcomeSummary;
    }
}
