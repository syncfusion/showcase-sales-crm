using SalesCrm.Domain.Common;
using SalesCrm.Domain.Enums;

namespace SalesCrm.Domain.Entities;

public sealed class Contact : EntityBase<Guid>
{
    public Contact(Guid id, Guid accountId, string firstName, string lastName, string email, ContactRoleType roleType)
        : base(id)
    {
        AccountId = accountId;
        FirstName = RequireText(firstName, nameof(firstName));
        LastName = RequireText(lastName, nameof(lastName));
        Email = RequireText(email, nameof(email));
        RoleType = roleType;
        RelationshipStatus = "Active";
    }

    public Guid AccountId { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string? JobTitle { get; private set; }

    public ContactRoleType RoleType { get; private set; }

    public string? Phone { get; private set; }

    public string RelationshipStatus { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateJobTitle(string? jobTitle) => JobTitle = jobTitle;

    public void UpdatePhone(string? phone) => Phone = phone;

    public void UpdateRelationshipStatus(string relationshipStatus) => RelationshipStatus = RequireText(relationshipStatus, nameof(relationshipStatus));

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
