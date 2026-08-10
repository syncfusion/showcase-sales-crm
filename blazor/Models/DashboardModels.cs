namespace SalesCrm.Blazor.Models;

/// <summary>Dashboard summary returned by GET /api/dashboard.</summary>
public sealed class DashboardResponse
{
    public DashboardSummaryDto Summary { get; set; } = new();
}

public sealed class DashboardSummaryDto
{
    public decimal Revenue { get; set; }
    public decimal PipelineValue { get; set; }
    public decimal WinRate { get; set; }
    public decimal WeightedForecast { get; set; }
    public List<RecentOpportunityDto> RecentOpportunities { get; set; } = new();
    public List<StageBreakdownDto> StageBreakdown { get; set; } = new();
}

public sealed class RecentOpportunityDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ExpectedCloseDate { get; set; }
}

public sealed class StageBreakdownDto
{
    public string Stage { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int Count { get; set; }
}
