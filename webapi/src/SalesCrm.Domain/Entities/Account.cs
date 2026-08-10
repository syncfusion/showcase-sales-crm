using SalesCrm.Domain.Common;
using SalesCrm.Domain.Enums;

namespace SalesCrm.Domain.Entities;

public sealed class Account : AuditableEntity<Guid>
{
    private readonly List<Contact> _contacts = new();
    private readonly List<Opportunity> _opportunities = new();
    private readonly List<Activity> _activities = new();

    public Account(Guid id, string name, string industry, Guid territoryId, Guid ownerUserId)
        : base(id)
    {
        Name = RequireText(name, nameof(name));
        Industry = RequireText(industry, nameof(industry));
        TerritoryId = territoryId;
        OwnerUserId = ownerUserId;
        Status = AccountStatus.Prospect;
    }

    public string Name { get; private set; }

    public string Industry { get; private set; }

    public Guid TerritoryId { get; private set; }

    public Guid OwnerUserId { get; private set; }

    public AccountStatus Status { get; private set; }

    public string? AnnualRevenueBand { get; private set; }

    public string? EmployeeBand { get; private set; }

    public string? PrimaryRegion { get; private set; }

    public DateTime? LastActivityOnUtc { get; private set; }

    public IReadOnlyCollection<Contact> Contacts => _contacts;

    public IReadOnlyCollection<Opportunity> Opportunities => _opportunities;

    public IReadOnlyCollection<Activity> Activities => _activities;

    public void UpdateStatus(AccountStatus status) => Status = status;

    public void UpdateSegments(string? annualRevenueBand, string? employeeBand, string? primaryRegion)
    {
        AnnualRevenueBand = annualRevenueBand;
        EmployeeBand = employeeBand;
        PrimaryRegion = primaryRegion;
    }

    public void TouchActivity(DateTime occurredOnUtc) => LastActivityOnUtc = occurredOnUtc;

    public void AddContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        _contacts.Add(contact);
    }

    public void AddOpportunity(Opportunity opportunity)
    {
        ArgumentNullException.ThrowIfNull(opportunity);
        _opportunities.Add(opportunity);
    }

    public void AddActivity(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        _activities.Add(activity);
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
