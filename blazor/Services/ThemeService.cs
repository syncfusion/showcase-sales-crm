using Microsoft.JSInterop;

namespace SalesCrm.Blazor.Services;

/// <summary>Manages the dark/light CRM theme persisted in localStorage and applied via data-theme on html.</summary>
public sealed class ThemeService
{
    private const string StorageKey = "sales-crm-theme";
    private readonly IJSRuntime js;
    private bool isDark;

    public event Action? ThemeChanged;

    public ThemeService(IJSRuntime js) => this.js = js;

    public bool IsDark => isDark;

    public async Task InitializeAsync()
    {
        var stored = await js.InvokeAsync<string>("localStorage.getItem", StorageKey);
        isDark = string.Equals(stored, "dark", StringComparison.OrdinalIgnoreCase);
        await ApplyThemeAsync();
    }

    public async Task ToggleAsync()
    {
        isDark = !isDark;
        await js.InvokeVoidAsync("localStorage.setItem", StorageKey, isDark ? "dark" : "light");
        await ApplyThemeAsync();
        ThemeChanged?.Invoke();
    }

    private async Task ApplyThemeAsync()
    {
        await js.InvokeVoidAsync("salesCrmTheme.apply", isDark);
    }
}
