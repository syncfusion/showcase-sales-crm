using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Admin;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/admin/seed")]
[Produces("application/json")]
[Tags("AdminSeed")]
public sealed class AdminSeedController : ControllerBase
{
    private readonly IAdminSeedService adminSeedService;

    public AdminSeedController(IAdminSeedService adminSeedService) => this.adminSeedService = adminSeedService;

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(SeedRefreshResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SeedRefreshResponse>> Refresh(CancellationToken cancellationToken)
        => Ok(await adminSeedService.RefreshAsync(cancellationToken));
}
