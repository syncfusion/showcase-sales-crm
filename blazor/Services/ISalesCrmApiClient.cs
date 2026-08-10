using SalesCrm.Blazor.Models;

namespace SalesCrm.Blazor.Services;

/// <summary>Abstraction over the Sales CRM Web API used by all Blazor pages.</summary>
public interface ISalesCrmApiClient
{
    Task<DashboardSummaryDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactDto>> GetContactsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OpportunityDto>> GetOpportunitiesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PipelineCardDto>> GetPipelineAsync(CancellationToken cancellationToken = default);
    Task<ForecastSummaryDto> GetForecastAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActivityDto>> GetActivitiesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TerritorySummaryDto>> GetTerritoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentTemplateDto>> GetDocumentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AiRecommendationDto>> GetAiRecommendationsAsync(CancellationToken cancellationToken = default);
    Task<SeedRefreshResponse> RefreshSeedDataAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateOpportunityStageAsync(string opportunityId, string stage, CancellationToken cancellationToken = default);
}
