using System.Globalization;

namespace SalesCrm.Blazor.Services;

/// <summary>Shared formatting helpers mirroring the React app formatters.</summary>
public static class Formatters
{
    private static readonly NumberFormatInfo UsNumber = CultureInfo.GetCultureInfo("en-US").NumberFormat;

    public static string FormatUsd(decimal? value)
        => (value ?? 0m).ToString("C", CultureInfo.GetCultureInfo("en-US"));

    public static string FormatCompactUsd(decimal? value)
    {
        var amount = (double)(value ?? 0m);
        return amount switch
        {
            >= 1_000_000 => $"${amount / 1_000_000:F1}M",
            >= 1_000 => $"${amount / 1_000:F1}K",
            _ => $"${amount:F0}"
        };
    }

    public static string FormatPercent(int? value)
        => ((value ?? 0) / 100.0).ToString("P0", CultureInfo.GetCultureInfo("en-US"));

    public static string FormatMonthDayYear(DateOnly? value)
        => value is null ? string.Empty : FormatMonthDayYear(value.Value);

    public static string FormatMonthDayYear(DateOnly value)
        => $"{value.Month:00}-{value.Day:00}-{value.Year}";

    public static string FormatMonthDayYear(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)) return FormatMonthDayYear(d);
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) return $"{dt.Month:00}-{dt.Day:00}-{dt.Year}";
        return value;
    }

    /// <summary>Converts a stage enum name (e.g. "ClosedWon") into a readable label ("Closed Won").</summary>
    public static string StageLabel(string stage)
        => stage switch
        {
            "Qualification" => "Qualification",
            "Discovery" => "Discovery",
            "Proposal" => "Proposal",
            "Negotiation" => "Negotiation",
            "ClosedWon" => "Closed Won",
            "ClosedLost" => "Closed Lost",
            _ => stage
        };

    /// <summary>Splits a PascalCase stage name into words (e.g. "ClosedWon" => "Closed Won").</summary>
    public static string SplitPascal(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var span = value.AsSpan();
        var builder = new System.Text.StringBuilder(span.Length + 4);
        builder.Append(span[0]);
        for (int i = 1; i < span.Length; i++)
        {
            if (char.IsUpper(span[i]) && !char.IsUpper(span[i - 1]))
            {
                builder.Append(' ');
            }
            builder.Append(span[i]);
        }
        return builder.ToString();
    }
}
