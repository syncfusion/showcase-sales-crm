using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class Territory : EntityBase<Guid>
{
    public Territory(Guid id, string name, string regionCode, Guid managerUserId, string coverageType)
        : base(id)
    {
        Name = RequireText(name, nameof(name));
        RegionCode = RequireText(regionCode, nameof(regionCode));
        ManagerUserId = managerUserId;
        CoverageType = RequireText(coverageType, nameof(coverageType));
        Status = "Active";
    }

    public string Name { get; private set; }

    public string RegionCode { get; private set; }

    public Guid ManagerUserId { get; private set; }

    public string CoverageType { get; private set; }

    public string Status { get; private set; }

    public void Rename(string name) => Name = RequireText(name, nameof(name));

    public void ChangeStatus(string status) => Status = RequireText(status, nameof(status));

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
