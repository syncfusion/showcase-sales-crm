using SalesCrm.Domain.Common;

namespace SalesCrm.Domain.Entities;

public sealed class SalesDocument : AuditableEntity<Guid>
{
    public SalesDocument(Guid id, Guid accountId, Guid? opportunityId, Guid authorUserId, string documentType, string title, string templateKey)
        : base(id)
    {
        AccountId = accountId;
        OpportunityId = opportunityId;
        AuthorUserId = authorUserId;
        DocumentType = RequireText(documentType, nameof(documentType));
        Title = RequireText(title, nameof(title));
        TemplateKey = RequireText(templateKey, nameof(templateKey));
    }

    public string DocumentType { get; private set; }

    public string Title { get; private set; }

    public Guid AccountId { get; private set; }

    public Guid? OpportunityId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string TemplateKey { get; private set; }

    public void Rename(string title) => Title = RequireText(title, nameof(title));

    public void Retarget(Guid accountId, Guid? opportunityId)
    {
        AccountId = accountId;
        OpportunityId = opportunityId;
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
