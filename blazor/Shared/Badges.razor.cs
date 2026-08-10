using Microsoft.AspNetCore.Components;
using SalesCrm.Blazor.Services;

namespace SalesCrm.Blazor.Shared;

/// <summary>Badge helpers mirroring the React Badges.tsx (status/score/probability/win-rate).</summary>
public static class Badges
{
    public static string NormalizeStatusClass(string? value)
    {
        var normalized = (value ?? string.Empty).Trim().ToLowerInvariant().Replace(" ", "");
        return normalized switch
        {
            "active" or "closedwon" or "won" => "status-active",
            "prospect" or "discovery" or "qualification" or "proposal" => "status-prospect",
            "atrisk" or "at-risk" or "negotiation" => "status-atrisk",
            "inactive" or "closedlost" or "lost" => "status-inactive",
            _ => string.Empty
        };
    }

    public static string ToneClassForScore(int score) => score switch
    {
        >= 75 => "metric-strong",
        >= 50 => "metric-medium",
        >= 30 => "metric-low",
        _ => "metric-risk"
    };

    public static string ToneClassForProbability(int probability) => probability switch
    {
        >= 75 => "metric-strong",
        >= 50 => "metric-medium",
        >= 30 => "metric-low",
        _ => "metric-risk"
    };
}
