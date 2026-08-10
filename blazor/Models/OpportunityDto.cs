namespace SalesCrm.Blazor.Models;

public sealed class OpportunityDto
{
    public string Id { get; set; } = string.Empty;
    public string OpportunityName { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Probability { get; set; }
    public DateOnly ExpectedCloseDate { get; set; }
    public string Owner { get; set; } = string.Empty;
}
