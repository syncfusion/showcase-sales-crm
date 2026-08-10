using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Contacts;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[Produces("application/json")]
[Tags("Contacts")]
public sealed class ContactsController : ControllerBase
{
    private readonly IContactService contactService;

    public ContactsController(IContactService contactService)
    {
        this.contactService = contactService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ContactListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContactListItemDto>>> Get(CancellationToken cancellationToken)
    {
        var contacts = await contactService.GetContactsAsync(cancellationToken);
        return Ok(contacts);
    }
}
