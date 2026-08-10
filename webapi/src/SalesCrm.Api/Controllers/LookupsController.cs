using Microsoft.AspNetCore.Mvc;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/lookups")]
[Produces("application/json")]
[Tags("Lookups")]
public sealed class LookupsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { lookups = Array.Empty<string>() });
}
