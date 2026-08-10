using Microsoft.AspNetCore.Components;

namespace SalesCrm.Blazor.Components.Shared;

/// <summary>
/// Renders a Lucide static SVG icon by name. Resolves the inner SVG markup from
/// <see cref="LucideIcons"/> and emits it as <see cref="MarkupString"/> so the
/// child elements (paths, rects, circles) are rendered as real SVG nodes rather
/// than HTML-encoded text.
/// </summary>
public partial class AppIcon : ComponentBase
{
    /// <summary>Icon name resolved against <see cref="LucideIcons.Icons"/>.</summary>
    [Parameter] public string? Name { get; set; }

    /// <summary>Pixel size for both width and height. Defaults to 20.</summary>
    [Parameter] public int Size { get; set; } = 20;

    /// <summary>Stroke width. Defaults to 1.9 to match the React app.</summary>
    [Parameter] public double StrokeWidth { get; set; } = 1.9;

    /// <summary>Additional CSS classes appended to <c>lucide-icon</c>.</summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>Optional accessible title rendered inside a <c>&lt;title&gt;</c> element.</summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>Optional aria-label for the SVG. Falls back to <see cref="Title"/>.</summary>
    [Parameter] public string? AriaLabel { get; set; }

    private string InnerMarkup => LucideIcons.GetMarkup(Name);
}
