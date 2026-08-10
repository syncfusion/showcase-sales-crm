namespace SalesCrm.Contracts.Documents;

public sealed record DocumentTemplateDto(string Id, string Title, string Type, string Status, string Owner, string Account, string Opportunity, string TemplateKey);
