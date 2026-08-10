namespace SalesCrm.Blazor.Models;

/// <summary>Account returned by GET /api/accounts. API returns "Name" as accountName.</summary>
public sealed class AccountDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int HealthScore { get; set; }
}
