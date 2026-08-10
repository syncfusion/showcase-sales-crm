using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Dashboard;
using SalesCrm.Contracts.Pipeline;
using SalesCrm.Contracts.Territories;
using Xunit;

namespace SalesCrm.ApiTests;

public sealed class ApiSmokeTests
{
    [Theory]
    [InlineData("/api/dashboard")]
    [InlineData("/api/pipeline/stages")]
    [InlineData("/api/territories")]
    [InlineData("/swagger/v1/swagger.json")]
    public async Task Get_ShouldReturnSuccess_ForShowcaseEndpoints(string path)
    {
        await using var factory = new SalesCrmApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DashboardEndpoint_ShouldReturnSummaryContract()
    {
        await using var factory = new SalesCrmApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetFromJsonAsync<DashboardResponse>("/api/dashboard");

        Assert.NotNull(response);
        Assert.Equal(250_000m, response.Summary.PipelineValue);
        Assert.Single(response.Summary.RecentOpportunities);
    }

    private sealed class SalesCrmApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDashboardService>();
                services.RemoveAll<IPipelineService>();
                services.RemoveAll<ITerritoryService>();

                services.AddScoped<IDashboardService, FakeDashboardService>();
                services.AddScoped<IPipelineService, FakePipelineService>();
                services.AddScoped<ITerritoryService, FakeTerritoryService>();
            });
        }
    }

    private sealed class FakeDashboardService : IDashboardService
    {
        public Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default)
        {
            var summary = new DashboardSummaryDto(
                100_000m,
                250_000m,
                50m,
                125_000m,
                [new RecentOpportunityDto("opp-1", "Contract Review", "Acme Cloud Systems", "Negotiation", 250_000m, new DateOnly(2026, 7, 1))],
                [new StageBreakdownDto("Negotiation", 250_000m, 1)]);

            return Task.FromResult(new DashboardResponse(summary));
        }
    }

    private sealed class FakePipelineService : IPipelineService
    {
        public Task<IReadOnlyList<PipelineCardDto>> GetStagesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PipelineCardDto>>(
                [new PipelineCardDto("opp-1", "Commit Negotiation", "Acme Cloud Systems", "Negotiation", 250_000m, "Commit", "Daniel Chen", new DateOnly(2026, 7, 1), 75, "High", 250_000m, 1, null, null, null, null)]);
    }

    private sealed class FakeTerritoryService : ITerritoryService
    {
        public Task<IReadOnlyList<TerritorySummaryDto>> GetTerritoriesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TerritorySummaryDto>>(
                [new TerritorySummaryDto("territory-1", "Great Lakes", "GLK", 250_000m, 4, 50, 41.8781, -87.6298)]);
    }
}
