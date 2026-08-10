namespace SalesCrm.Contracts.Opportunities;

public sealed record OpportunityListItemDto(string Id, string OpportunityName, string Account, string Stage, decimal Amount, int Probability, DateOnly ExpectedCloseDate, string Owner);
