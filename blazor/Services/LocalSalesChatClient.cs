using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using SalesCrm.Blazor.Models;

namespace SalesCrm.Blazor.Services;

/// <summary>
/// Local, offline <see cref="IChatClient"/> that mirrors the React app's
/// <c>buildDealRiskResponse</c> rule-based responder. Registered as the default
/// chat client so the AI Assistant works without any external LLM/API key.
/// Swap with a real client (OpenAI/Azure/Ollama) by registering a different
/// <see cref="IChatClient"/> in Program.cs.
/// </summary>
public sealed class LocalSalesChatClient : IChatClient
{
    private readonly ISalesCrmApiClient apiClient;

    public LocalSalesChatClient(ISalesCrmApiClient apiClient) => this.apiClient = apiClient;

    public ChatClientMetadata Metadata { get; } = new("local-sales-chat", default, "1.0");

    public void Dispose() { }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var prompt = chatMessages.LastOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;
        var recommendations = await apiClient.GetAiRecommendationsAsync(cancellationToken);
        var response = BuildDealRiskResponse(recommendations, prompt);
        return new ChatResponse(new ChatMessage(ChatRole.Assistant, response));
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var prompt = chatMessages.LastOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;
        var recommendations = await apiClient.GetAiRecommendationsAsync(cancellationToken);
        var response = BuildDealRiskResponse(recommendations, prompt);
        // Emit the response in small chunks to simulate streaming.
        var chunkSize = 80;
        for (var i = 0; i < response.Length; i += chunkSize)
        {
            await Task.Delay(20, cancellationToken);
            yield return new ChatResponseUpdate(ChatRole.Assistant, response.Substring(i, Math.Min(chunkSize, response.Length - i)));
        }
    }

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceType == typeof(IChatClient) ? this : null;

    /// <summary>
    /// Replicates <c>buildDealRiskResponse</c> from the React app's AiAssistantPage,
    /// with branches for "email" prompts, "mutual action" prompts, and the default
    /// pipeline risk summary. Public so pages can pre-seed prompts without a round-trip.
    /// </summary>
    public static string BuildDealRiskResponse(IReadOnlyList<AiRecommendationDto> items, string prompt)
    {
        var lower = (prompt ?? string.Empty).ToLowerInvariant();
        var take = lower.Contains("which") ? 5 : 3;
        var candidates = (items ?? Array.Empty<AiRecommendationDto>()).Take(take).ToList();

        if (candidates.Count == 0)
        {
            return "No AI recommendations are currently available. Refresh the data via Admin → Seed Data and try again.";
        }

        var total = candidates.Sum(x => x.Amount);

        if (lower.Contains("email"))
        {
            var item = candidates[0];
            return $"Subject: Aligning next steps for {item.RelatedRecord}{Environment.NewLine}{Environment.NewLine}" +
                   $"Hi team,{Environment.NewLine}{Environment.NewLine}" +
                   $"I wanted to follow up on {item.RelatedRecord}. Based on the current close date of {item.CloseDate}, " +
                   $"the highest-value next step is to {item.Title.ToLowerInvariant()}.{Environment.NewLine}{Environment.NewLine}" +
                   $"The main signal I am watching is: {item.Reason}{Environment.NewLine}{Environment.NewLine}" +
                   $"Could we confirm the owner for the next decision milestone and align on the mutual action plan this week?{Environment.NewLine}{Environment.NewLine}" +
                   $"Best,{Environment.NewLine}Sales Team";
        }

        if (lower.Contains("mutual action"))
        {
            var item = candidates[0];
            return $"Mutual Action Plan for {item.RelatedRecord}{Environment.NewLine}{Environment.NewLine}" +
                   "1. Confirm business case and success metrics." + Environment.NewLine +
                   "2. Assign security, legal, and procurement owners." + Environment.NewLine +
                   "3. Review implementation milestones and target go-live." + Environment.NewLine +
                   $"4. Finalize commercial terms before {item.CloseDate}." + Environment.NewLine +
                   "5. Executive approval and signature." + Environment.NewLine + Environment.NewLine +
                   $"Recommended action: {item.Title}.";
        }

        var lines = candidates.Select((item, index) =>
            $"{index + 1}. {item.RelatedRecord} ({item.Priority}, {Formatters.FormatUsd(item.Amount)}, closes {item.CloseDate})" + Environment.NewLine +
            $"   - Signal: {item.Reason}" + Environment.NewLine +
            $"   - Evidence: {item.SupportingEvidence}" + Environment.NewLine +
            $"   - Next best action: {item.Title}");

        return $"Pipeline risk summary for {candidates.Count} deal{(candidates.Count == 1 ? string.Empty : "s")} totaling {Formatters.FormatUsd(total)}:" +
               Environment.NewLine + Environment.NewLine +
               string.Join(Environment.NewLine + Environment.NewLine, lines) + Environment.NewLine + Environment.NewLine +
               "Recommendation: prioritize the highest-value High priority deal first, then update the forecast category after the next confirmed buyer action.";
    }
}
