using SalesCrm.Domain.Common;
using SalesCrm.Domain.ValueObjects;

namespace SalesCrm.Domain.Entities;

public sealed class Forecast : EntityBase<Guid>
{
    private Forecast() : base(Guid.Empty)
    {
        ForecastCategory = string.Empty;
        PeriodKey = string.Empty;
        CommittedAmount = null!;
        BestCaseAmount = null!;
        PipelineAmount = null!;
    }

    public Forecast(Guid id, Guid territoryId, Guid? ownerUserId, string forecastCategory, Money committedAmount, Money bestCaseAmount, Money pipelineAmount, string periodKey)
        : base(id)
    {
        TerritoryId = territoryId;
        OwnerUserId = ownerUserId;
        ForecastCategory = RequireText(forecastCategory, nameof(forecastCategory));
        CommittedAmount = committedAmount;
        BestCaseAmount = bestCaseAmount;
        PipelineAmount = pipelineAmount;
        PeriodKey = RequireText(periodKey, nameof(periodKey));
    }

    public Guid TerritoryId { get; private set; }

    public Guid? OwnerUserId { get; private set; }

    public string ForecastCategory { get; private set; }

    public Money CommittedAmount { get; private set; }

    public Money BestCaseAmount { get; private set; }

    public Money PipelineAmount { get; private set; }

    public string PeriodKey { get; private set; }

    public DateTime SnapshotTakenOnUtc { get; private set; }

    public void MarkSnapshotTaken(DateTime snapshotTakenOnUtc) => SnapshotTakenOnUtc = snapshotTakenOnUtc;

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
