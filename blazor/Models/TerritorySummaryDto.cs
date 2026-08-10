namespace SalesCrm.Blazor.Models;

public sealed class TerritorySummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Accounts { get; set; }
    public int WinRate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
