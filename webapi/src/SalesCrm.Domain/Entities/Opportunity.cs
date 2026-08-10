using SalesCrm.Domain.Common;
using SalesCrm.Domain.Enums;
using SalesCrm.Domain.ValueObjects;

namespace SalesCrm.Domain.Entities;

public sealed class Opportunity : AuditableEntity<Guid>
{
    private readonly List<Activity> _activities = new();
    private readonly List<SalesDocument> _salesDocuments = new();
    private readonly List<AiRecommendation> _aiRecommendations = new();

    private Opportunity()
        : base(Guid.Empty)
    {
        ForecastCategory = string.Empty;
        RiskLevel = string.Empty;
        EstimatedValue = null!;
    }

    public Opportunity(Guid id, Guid accountId, Guid? primaryContactId, Guid ownerUserId, OpportunityStage stage, string forecastCategory, Money estimatedValue, int probabilityPercent, DateTime expectedCloseDate)
        : base(id)
    {
        AccountId = accountId;
        PrimaryContactId = primaryContactId;
        OwnerUserId = ownerUserId;
        Stage = stage;
        ForecastCategory = RequireText(forecastCategory, nameof(forecastCategory));
        EstimatedValue = estimatedValue;
        SetProbability(probabilityPercent);
        ExpectedCloseDate = expectedCloseDate;
        RiskLevel = "Medium";
    }

    public Guid AccountId { get; private set; }

    public Guid? PrimaryContactId { get; private set; }

    public Guid OwnerUserId { get; private set; }

    public OpportunityStage Stage { get; private set; }

    public string ForecastCategory { get; private set; }

    public Money EstimatedValue { get; private set; }

    public int ProbabilityPercent { get; private set; }

    public DateTime ExpectedCloseDate { get; private set; }

    public string? NextAction { get; private set; }

    public string RiskLevel { get; private set; }

    public IReadOnlyCollection<Activity> Activities => _activities;

    public IReadOnlyCollection<SalesDocument> SalesDocuments => _salesDocuments;

    public IReadOnlyCollection<AiRecommendation> AiRecommendations => _aiRecommendations;

    public void MoveToStage(OpportunityStage stage) => Stage = stage;

    public void UpdateNextAction(string? nextAction) => NextAction = nextAction;

    public void UpdateRiskLevel(string riskLevel) => RiskLevel = RequireText(riskLevel, nameof(riskLevel));

    public void SetProbability(int probabilityPercent)
    {
        if (probabilityPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(probabilityPercent));
        }

        ProbabilityPercent = probabilityPercent;
    }

    public void AddActivity(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        _activities.Add(activity);
    }

    public void AddSalesDocument(SalesDocument salesDocument)
    {
        ArgumentNullException.ThrowIfNull(salesDocument);
        _salesDocuments.Add(salesDocument);
    }

    public void AddRecommendation(AiRecommendation recommendation)
    {
        ArgumentNullException.ThrowIfNull(recommendation);
        _aiRecommendations.Add(recommendation);
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
