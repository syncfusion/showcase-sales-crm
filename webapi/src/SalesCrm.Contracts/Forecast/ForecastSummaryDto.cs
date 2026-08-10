namespace SalesCrm.Contracts.Forecast;

public sealed record ForecastSummaryDto(decimal Commit, decimal BestCase, decimal Pipeline, decimal Weighted, IReadOnlyList<ForecastTrendPointDto> Trend);

public sealed record ForecastTrendPointDto(string Period, decimal Commit, decimal BestCase, decimal Pipeline, decimal Weighted);
