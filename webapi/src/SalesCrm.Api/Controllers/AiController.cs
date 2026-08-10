using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Ai;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/ai")]
[Produces("application/json")]
[Tags("AI")]
public sealed class AiController : ControllerBase
{
    private readonly IAiRecommendationService aiRecommendationService;

    public AiController(IAiRecommendationService aiRecommendationService)
    {
        this.aiRecommendationService = aiRecommendationService;
    }

    [HttpGet("recommendations")]
    [ProducesResponseType(typeof(IReadOnlyList<AiRecommendationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AiRecommendationDto>>> GetRecommendations(CancellationToken cancellationToken)
    {
        var recommendations = await aiRecommendationService.GetRecommendationsAsync(cancellationToken);
        return Ok(recommendations);
    }
}
