namespace SalesCrm.Blazor.Models;

public sealed class AiRecommendationDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string SupportingEvidence { get; set; } = string.Empty;
    public string RelatedRecord { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CloseDate { get; set; } = string.Empty;
}

public sealed class SeedRefreshResponse
{
    public string Status { get; set; } = string.Empty;
}
