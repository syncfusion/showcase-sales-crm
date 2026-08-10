using Microsoft.AspNetCore.Components;

namespace SalesCrm.Blazor.Shared;

/// <summary>Page header with an optional icon box, title, and subtitle (mirrors React PageHeader).</summary>
public partial class PageHeader : ComponentBase
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Subtitle { get; set; } = string.Empty;
    [Parameter] public string? Icon { get; set; }
}
