namespace SalesCrm.Contracts.Ai;

public sealed record AiRecommendationDto(string Id, string Title, string Reason, string Priority, string SupportingEvidence, string RelatedRecord, decimal Amount, string CloseDate);
