using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Opportunities;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
[Produces("application/json")]
[Tags("Opportunities")]
public sealed class OpportunitiesController : ControllerBase
{
    private readonly IOpportunityService opportunityService;

    public OpportunitiesController(IOpportunityService opportunityService)
    {
        this.opportunityService = opportunityService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OpportunityListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OpportunityListItemDto>>> Get(CancellationToken cancellationToken)
    {
        var opportunities = await opportunityService.GetOpportunitiesAsync(cancellationToken);
        return Ok(opportunities);
    }

    [HttpPut("{opportunityId:guid}/stage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStage(
        Guid opportunityId,
        [FromBody] UpdateOpportunityStageRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Stage))
        {
            return BadRequest(new { error = "Stage is required." });
        }

        try
        {
            await opportunityService.UpdateStageAsync(opportunityId, request.Stage, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        return NoContent();
    }
}
