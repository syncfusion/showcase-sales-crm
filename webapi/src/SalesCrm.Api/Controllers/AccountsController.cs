using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Accounts;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Produces("application/json")]
[Tags("Accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAccountService accountService;

    public AccountsController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccountListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AccountListItemDto>>> Get(CancellationToken cancellationToken)
    {
        var accounts = await accountService.GetAccountsAsync(cancellationToken);
        return Ok(accounts);
    }
}
