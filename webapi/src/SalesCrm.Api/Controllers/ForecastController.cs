using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Forecast;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/forecast")]
[Produces("application/json")]
[Tags("Forecast")]
public sealed class ForecastController : ControllerBase
{
    private readonly IForecastService forecastService;

    public ForecastController(IForecastService forecastService)
    {
        this.forecastService = forecastService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ForecastSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ForecastSummaryDto>> Get(CancellationToken cancellationToken)
    {
        var forecasts = await forecastService.GetForecastAsync(cancellationToken);
        return Ok(forecasts);
    }
}
