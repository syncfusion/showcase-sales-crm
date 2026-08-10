namespace SalesCrm.Blazor.Models;

/// <summary>Pipeline card returned by GET /api/pipeline/stages.</summary>
public sealed class PipelineCardDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ForecastCategory { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateOnly ExpectedCloseDate { get; set; }
    public int Probability { get; set; }
    public string Priority { get; set; } = "Medium";
    public decimal StageTotal { get; set; }
    public int StageCount { get; set; }
    public DateOnly? WonDate { get; set; }
    public DateOnly? LostDate { get; set; }
    public string? LossReason { get; set; }
    public int? SalesCycleDays { get; set; }
}

/// <summary>Request body for PUT /api/opportunities/{id}/stage.</summary>
public sealed class UpdateOpportunityStageRequest
{
    public string Stage { get; set; } = string.Empty;
}
