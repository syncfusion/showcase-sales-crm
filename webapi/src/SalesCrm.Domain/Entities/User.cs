using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class User : EntityBase<Guid>
{
    public User(Guid id, string displayName, string email)
        : base(id)
    {
        DisplayName = RequireText(displayName, nameof(displayName));
        Email = RequireText(email, nameof(email));
        IsActive = true;
    }

    public string DisplayName { get; private set; }

    public string Email { get; private set; }

    public bool IsActive { get; private set; }

    public void Rename(string displayName) => DisplayName = RequireText(displayName, nameof(displayName));

    public void Deactivate() => IsActive = false;

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
