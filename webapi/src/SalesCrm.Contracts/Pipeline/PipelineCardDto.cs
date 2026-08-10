namespace SalesCrm.Contracts.Pipeline;

public sealed record PipelineCardDto(
    string Id,
    string Title,
    string Account,
    string Stage,
    decimal Amount,
    string ForecastCategory,
    string Owner,
    DateOnly ExpectedCloseDate,
    int Probability,
    string Priority,
    decimal StageTotal,
    int StageCount,
    DateOnly? WonDate = null,
    DateOnly? LostDate = null,
    string? LossReason = null,
    int? SalesCycleDays = null);
