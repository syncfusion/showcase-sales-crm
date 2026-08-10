namespace SalesCrm.Contracts.Accounts;

public sealed record AccountListItemDto(string Id, string Name, string Industry, string Region, string Status, string Owner, decimal Revenue, int HealthScore);
