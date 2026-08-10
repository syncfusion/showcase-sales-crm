namespace SalesCrm.Contracts.Opportunities;

/// <summary>Request body for updating the stage of an opportunity (used by Kanban drag-and-drop).</summary>
public sealed record UpdateOpportunityStageRequest(string Stage);
