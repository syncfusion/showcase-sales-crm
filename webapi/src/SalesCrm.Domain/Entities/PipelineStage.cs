using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class PipelineStage : EntityBase<Guid>
{
    private PipelineStage()
        : base(Guid.Empty)
    {
        Name = string.Empty;
    }

    public PipelineStage(Guid id, string name, int sortOrder, bool isClosedStage)
        : base(id)
    {
        Name = RequireText(name, nameof(name));
        SortOrder = sortOrder;
        IsClosedStage = isClosedStage;
    }

    public string Name { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsClosedStage { get; private set; }

    public void Rename(string name) => Name = RequireText(name, nameof(name));

    public void Reorder(int sortOrder) => SortOrder = sortOrder;

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
