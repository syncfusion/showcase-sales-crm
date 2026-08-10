using SalesCrm.Application.Services;
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.ValueObjects;
using SalesCrm.UnitTests.TestDoubles;
using Xunit;

namespace SalesCrm.UnitTests.Application;

public sealed class ForecastServiceTests
{
    [Fact]
    public async Task GetForecastAsync_ShouldGroupTrendByPeriodInOrder()
    {
        var forecasts = new[]
        {
            CreateForecast("2026-02", 100_000m),
            CreateForecast("2026-01", 50_000m),
            CreateForecast("2026-01", 25_000m)
        };
        var service = new ForecastService(new InMemoryReadRepository<Forecast, Guid>(forecasts, item => item.Id));

        var result = await service.GetForecastAsync(TestContext.Current.CancellationToken);

        Assert.Equal(175_000m, result.Pipeline);
        Assert.Collection(result.Trend,
            period =>
            {
                Assert.Equal("2026-01", period.Period);
                Assert.Equal(75_000m, period.Pipeline);
            },
            period =>
            {
                Assert.Equal("2026-02", period.Period);
                Assert.Equal(100_000m, period.Pipeline);
            });
    }

    private static Forecast CreateForecast(string period, decimal pipeline)
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Pipeline", new Money(pipeline * 0.6m, "USD"), new Money(pipeline * 0.8m, "USD"), new Money(pipeline, "USD"), period);
}
