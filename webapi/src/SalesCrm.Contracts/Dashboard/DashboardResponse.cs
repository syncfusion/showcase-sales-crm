namespace SalesCrm.Contracts.Dashboard;

public sealed record DashboardResponse(DashboardSummaryDto Summary);
public sealed record DashboardSummaryDto(decimal Revenue, decimal PipelineValue, decimal WinRate, decimal WeightedForecast, IReadOnlyList<RecentOpportunityDto> RecentOpportunities, IReadOnlyList<StageBreakdownDto> StageBreakdown);
public sealed record RecentOpportunityDto(string Id, string Name, string Account, string Stage, decimal Amount, DateOnly ExpectedCloseDate);
public sealed record StageBreakdownDto(string Stage, decimal Value, int Count);
