namespace SalesCrm.Contracts.Activities;

public sealed record ActivityListItemDto(string Id, string Subject, string Account, DateTime StartTime, DateTime EndTime, string Status, string Owner);
