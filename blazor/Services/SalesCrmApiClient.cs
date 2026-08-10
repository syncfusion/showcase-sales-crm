using System.Net.Http.Json;
using SalesCrm.Blazor.Models;

namespace SalesCrm.Blazor.Services;

/// <summary>Reusable typed HTTP client for the Sales CRM Web API.</summary>
public sealed class SalesCrmApiClient : ISalesCrmApiClient
{
    private readonly HttpClient http;
    private readonly ILogger<SalesCrmApiClient> logger;

    public SalesCrmApiClient(HttpClient http, ILogger<SalesCrmApiClient> logger)
    {
        this.http = http;
        this.logger = logger;
    }

    public async Task<DashboardSummaryDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var response = await http.GetFromJsonAsync<DashboardResponse>("api/dashboard", cancellationToken)
            ?? new DashboardResponse();
        return response.Summary ?? new DashboardSummaryDto();
    }

    public async Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<AccountDto>("api/accounts", "accountName", cancellationToken);
    }

    public async Task<IReadOnlyList<ContactDto>> GetContactsAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<ContactDto>("api/contacts", null, cancellationToken);
    }

    public async Task<IReadOnlyList<OpportunityDto>> GetOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<OpportunityDto>("api/opportunities", null, cancellationToken);
    }

    public async Task<IReadOnlyList<PipelineCardDto>> GetPipelineAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<PipelineCardDto>("api/pipeline/stages", null, cancellationToken);
    }

    public async Task<ForecastSummaryDto> GetForecastAsync(CancellationToken cancellationToken = default)
    {
        var forecast = await http.GetFromJsonAsync<ForecastSummaryDto>("api/forecast", cancellationToken)
            ?? new ForecastSummaryDto();
        forecast.Trend ??= new List<ForecastTrendPointDto>();
        return forecast;
    }

    public async Task<IReadOnlyList<ActivityDto>> GetActivitiesAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<ActivityDto>("api/activities", null, cancellationToken);
    }

    public async Task<IReadOnlyList<TerritorySummaryDto>> GetTerritoriesAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<TerritorySummaryDto>("api/territories", null, cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentTemplateDto>> GetDocumentsAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<DocumentTemplateDto>("api/documents", null, cancellationToken);
    }

    public async Task<IReadOnlyList<AiRecommendationDto>> GetAiRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        return await GetListOrItemsAsync<AiRecommendationDto>("api/ai/recommendations", null, cancellationToken);
    }

    public async Task<SeedRefreshResponse> RefreshSeedDataAsync(CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync<object?>("api/admin/seed/refresh", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SeedRefreshResponse>(cancellationToken: cancellationToken)
            ?? new SeedRefreshResponse { Status = "Unknown" };
    }

    /// <summary>Updates only the opportunity stage after Kanban drag-and-drop.</summary>
    public async Task<bool> UpdateOpportunityStageAsync(string opportunityId, string stage, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new UpdateOpportunityStageRequest { Stage = stage };
            var response = await http.PutAsJsonAsync($"api/opportunities/{opportunityId}/stage", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update opportunity {Id} stage to {Stage}", opportunityId, stage);
            return false;
        }
    }

    /// <summary>
    /// Handles both bare-array and { items: [] } API response shapes and normalizes AccountDto.Name
    /// so the Blazor account grid always shows the accountName.
    /// </summary>
    private async Task<IReadOnlyList<T>> GetListOrItemsAsync<T>(string requestUri, string? altNameField, CancellationToken cancellationToken)
        where T : class
    {
        IReadOnlyList<T> items;
        try
        {
            var direct = await http.GetFromJsonAsync<List<T>>(requestUri, cancellationToken);
            items = direct ?? new List<T>();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogInformation(ex, "Direct list parse failed for {Uri}; trying enveloped response.", requestUri);
            try
            {
                var enveloped = await http.GetFromJsonAsync<ItemsEnvelope<T>>(requestUri, cancellationToken);
                items = enveloped?.Items ?? new List<T>();
            }
            catch (Exception inner) when (inner is not OperationCanceledException)
            {
                logger.LogError(inner, "Unable to load {Uri}", requestUri);
                throw new InvalidOperationException($"Unable to load data from {requestUri}.", inner);
            }
        }

        if (altNameField is not null && typeof(T) == typeof(AccountDto))
        {
            foreach (var item in items)
            {
                var account = (AccountDto)(object)item;
                if (string.IsNullOrWhiteSpace(account.Name) && !string.IsNullOrWhiteSpace(account.Industry))
                {
                    account.Name = $"Account {account.Id[..Math.Min(8, account.Id.Length)]}";
                }
            }
        }

        return items;
    }

    private sealed class ItemsEnvelope<T>
    {
        public List<T>? Items { get; set; }
    }
}
