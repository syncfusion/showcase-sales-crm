namespace SalesCrm.Contracts.Common;

public sealed record ListResponse<T>(IReadOnlyList<T> Items);
