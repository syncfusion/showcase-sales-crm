using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Dashboard;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
[Tags("Dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService dashboardService;

    public DashboardController(IDashboardService dashboardService) => this.dashboardService = dashboardService;

    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> Get(CancellationToken cancellationToken)
        => Ok(await dashboardService.GetDashboardAsync(cancellationToken));
}
