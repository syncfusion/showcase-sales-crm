using SalesCrm.Domain.Common;
using SalesCrm.Domain.ValueObjects;

namespace SalesCrm.Domain.Entities;

public sealed class Quote : AuditableEntity<Guid>
{
    private Quote() : base(Guid.Empty)
{
    QuoteNumber = string.Empty;
        TotalAmount = Money.Zero;
    Status = "Draft";
}

    public Quote(Guid id, Guid accountId, Guid opportunityId, Guid authorUserId, string quoteNumber, Money totalAmount)
        : base(id)
    {
        AccountId = accountId;
        OpportunityId = opportunityId;
        AuthorUserId = authorUserId;
        QuoteNumber = RequireText(quoteNumber, nameof(quoteNumber));
        TotalAmount = totalAmount;
        Status = "Draft";
    }

    public Guid AccountId { get; private set; }

    public Guid OpportunityId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string QuoteNumber { get; private set; }

    public Money TotalAmount { get; private set; }

    public string Status { get; private set; }

    public void Submit() => Status = "Submitted";

    public void Approve() => Status = "Approved";

    public void Reject() => Status = "Rejected";

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}
