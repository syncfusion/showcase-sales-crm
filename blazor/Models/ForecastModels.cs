namespace SalesCrm.Blazor.Models;

public sealed class ForecastSummaryDto
{
    public decimal Commit { get; set; }
    public decimal BestCase { get; set; }
    public decimal Pipeline { get; set; }
    public decimal Weighted { get; set; }
    public List<ForecastTrendPointDto> Trend { get; set; } = new();
}

public sealed class ForecastTrendPointDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Commit { get; set; }
    public decimal BestCase { get; set; }
    public decimal Pipeline { get; set; }
    public decimal Weighted { get; set; }
}
