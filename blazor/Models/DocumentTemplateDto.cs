namespace SalesCrm.Blazor.Models;

public sealed class DocumentTemplateDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string Opportunity { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
}
