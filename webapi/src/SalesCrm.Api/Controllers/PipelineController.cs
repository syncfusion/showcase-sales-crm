using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Pipeline;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/pipeline")]
[Produces("application/json")]
[Tags("Pipeline")]
public sealed class PipelineController : ControllerBase
{
    private readonly IPipelineService pipelineService;

    public PipelineController(IPipelineService pipelineService)
    {
        this.pipelineService = pipelineService;
    }

    [HttpGet("stages")]
    [ProducesResponseType(typeof(IReadOnlyList<PipelineCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PipelineCardDto>>> GetStages(CancellationToken cancellationToken)
    {
        var stages = await pipelineService.GetStagesAsync(cancellationToken);
        return Ok(stages);
    }
}
