using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Activities;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/activities")]
[Produces("application/json")]
[Tags("Activities")]
public sealed class ActivitiesController : ControllerBase
{
    private readonly IActivityService activityService;

    public ActivitiesController(IActivityService activityService)
    {
        this.activityService = activityService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ActivityListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ActivityListItemDto>>> Get(CancellationToken cancellationToken)
    {
        var activities = await activityService.GetActivitiesAsync(cancellationToken);
        return Ok(activities);
    }
}
