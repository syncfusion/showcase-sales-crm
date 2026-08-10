using Microsoft.AspNetCore.Mvc;
using SalesCrm.Application.Services;
using SalesCrm.Contracts.Documents;

namespace SalesCrm.Api.Controllers;

[ApiController]
[Route("api/documents")]
[Produces("application/json")]
[Tags("Documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentService documentService;

    public DocumentsController(IDocumentService documentService)
    {
        this.documentService = documentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DocumentTemplateDto>>> Get(CancellationToken cancellationToken)
    {
        var documents = await documentService.GetDocumentsAsync(cancellationToken);
        return Ok(documents);
    }
}
