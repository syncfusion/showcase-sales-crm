using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Territories;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/territories")]
[Produces("application/json")]
[Tags("Territories")]
public sealed class TerritoriesController : ControllerBase
{
    private readonly ITerritoryService territoryService;

    public TerritoriesController(ITerritoryService territoryService)
    {
        this.territoryService = territoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TerritorySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TerritorySummaryDto>>> Get(CancellationToken cancellationToken)
    {
        var territories = await territoryService.GetTerritoriesAsync(cancellationToken);
        return Ok(territories);
    }
}
