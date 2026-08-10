using System.Collections.Generic;

namespace SalesCrm.Blazor.Components.Shared;

/// <summary>
/// Central registry of Lucide static SVG inner markup, keyed by a short app
/// icon name. Each entry is the raw SVG child element markup (paths, rects,
/// circles, etc.) that is placed inside an <c>&lt;svg viewBox="0 0 24 24"&gt;</c>
/// wrapper. The markup uses single-quoted attributes so it can be embedded in
/// C# verbatim strings without escaping.
///
/// Source: https://lucide.dev/guide/static/ (icons inlined).
/// </summary>
internal static class LucideIcons
{
    /// <summary>
    /// Maps an app icon name to the inner SVG markup. Inner elements use
    /// single-quoted attribute values so they can be rendered verbatim via
    /// <see cref="Microsoft.AspNetCore.Components.MarkupString"/>.
    /// </summary>
    public static readonly Dictionary<string, string> Icons = new()
    {
        // ---- Sidebar / navigation ----
        { "dashboard", "<rect width='7' height='9' x='3' y='3' rx='1' /><rect width='7' height='5' x='14' y='3' rx='1' /><rect width='7' height='9' x='14' y='12' rx='1' /><rect width='7' height='5' x='3' y='16' rx='1' />" },
        { "accounts", "<path d='M3 21h18' /><path d='M5 21V7l8-4v18' /><path d='M19 21V11l-6-4' /><path d='M9 9v.01' /><path d='M9 13v.01' /><path d='M9 17v.01' />" },
        { "contacts", "<path d='M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2' /><circle cx='9' cy='7' r='4' /><path d='M22 21v-2a4 4 0 0 0-3-3.87' /><path d='M16 3.13a4 4 0 0 1 0 7.75' />" },
        { "opportunities", "<path d='M15 14c.2-1 .7-1.7 1.5-2.5 1-.9 1.5-2.2 1.5-3.5A6 6 0 0 0 6 8c0 1 .2 2.2 1.5 3.5.7.7 1.3 1.5 1.5 2.5'/><path d='M9 18h6'/><path d='M10 22h4'/>" },
        { "pipeline", "<rect width='7' height='9' x='3' y='3' rx='1' /><rect width='7' height='5' x='14' y='3' rx='1' /><rect width='7' height='9' x='14' y='12' rx='1' /><rect width='7' height='5' x='3' y='16' rx='1' />" },
        { "forecast", "<path d='M12 2v20' /><path d='m17 5-5-3-5 3' /><path d='m17 19-5 3-5-3' /><path d='M2 12h20' /><path d='m5 7-3 5 3 5' /><path d='m19 7 3 5-3 5' />" },
        { "activities", "<path d='M3 12h4l3-9 4 18 3-9h4' />" },
        { "territories", "<path d='M20 10c0 4.993-5.539 10.193-7.399 11.799a1 1 0 0 1-1.202 0C9.539 20.193 4 14.993 4 10a8 8 0 0 1 16 0' /><circle cx='12' cy='10' r='3' />" },
        { "documents", "<path d='M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z' /><path d='M14 2v4a2 2 0 0 0 2 2h4' /><path d='M10 9H8' /><path d='M16 13H8' /><path d='M16 17H8' />" },
        { "assistant", "<path d='M12 8V4H8' /><rect width='16' height='12' x='4' y='8' rx='2' /><path d='M2 14h2' /><path d='M20 14h2' /><path d='M15 13v2' /><path d='M9 13v2' />" },
        { "admin", "<ellipse cx='12' cy='5' rx='9' ry='3' /><path d='M3 5V19A9 3 0 0 0 21 19V5' /><path d='M3 12A9 3 0 0 0 21 12' />" },

        // ---- Dashboard metric icons ----
        { "revenue", "<path d='M3.85 8.62a4 4 0 0 1 4.78-4.77 4 4 0 0 1 6.74 0 4 4 0 0 1 4.78 4.78 4 4 0 0 1 0 6.74 4 4 0 0 1-4.77 4.78 4 4 0 0 1-6.75 0 4 4 0 0 1-4.78-4.77 4 4 0 0 1 0-6.76Z'/><path d='M16 8h-6a2 2 0 1 0 0 4h4a2 2 0 1 1 0 4H8'/><path d='M12 18V6'/>" },
        { "work", "<rect width='20' height='14' x='2' y='7' rx='2' ry='2' /><path d='M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16' />" },
        { "target", "<circle cx='12' cy='12' r='10' /><circle cx='12' cy='12' r='6' /><circle cx='12' cy='12' r='2' />" },
        { "users", "<path d='M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2' /><circle cx='9' cy='7' r='4' /><path d='M22 21v-2a4 4 0 0 0-3-3.87' /><path d='M16 3.13a4 4 0 0 1 0 7.75' />" },
        { "trending-up", "<path d='M16 7h6v6' /><path d='m22 7-8.5 8.5-5-5L2 17' />" },
        { "briefcase", "<rect width='20' height='14' x='2' y='7' rx='2' ry='2' /><path d='M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16' />" },

        // ---- Action / state icons ----
        { "sun", "<circle cx='12' cy='12' r='4' /><path d='M12 2v2' /><path d='M12 20v2' /><path d='m4.93 4.93 1.41 1.41' /><path d='m17.66 17.66 1.41 1.41' /><path d='M2 12h2' /><path d='M20 12h2' /><path d='m6.34 17.66-1.41 1.41' /><path d='m19.07 4.93-1.41 1.41' />" },
        { "moon", "<path d='M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z' />" },
        { "refresh", "<path d='M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8' /><path d='M21 3v5h-5' /><path d='M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16' /><path d='M8 16H3v5' />" },
        { "map-pin", "<path d='M20 10c0 4.993-5.539 10.193-7.399 11.799a1 1 0 0 1-1.202 0C9.539 20.193 4 14.993 4 10a8 8 0 0 1 16 0' /><circle cx='12' cy='10' r='3' />" },
        { "alert", "<path d='m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z' /><path d='M12 9v4' /><path d='M12 17h.01' />" },
        { "search", "<circle cx='11' cy='11' r='8' /><path d='m21 21-4.3-4.3' />" },
        { "check", "<path d='M20 6 9 17l-5-5' />" },
        { "x", "<path d='M18 6 6 18' /><path d='m6 6 12 12' />" },
        { "menu", "<line x1='4' x2='20' y1='6' y2='6' /><line x1='4' x2='20' y1='12' y2='12' /><line x1='4' x2='20' y1='18' y2='18' />" },
    };

    /// <summary>Returns the raw inner SVG markup for the named icon (empty string if unknown).</summary>
    public static string GetMarkup(string? name)
        => string.IsNullOrEmpty(name) || !Icons.TryGetValue(name, out var markup) ? string.Empty : markup;
}
