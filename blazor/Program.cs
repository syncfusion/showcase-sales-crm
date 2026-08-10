using SalesCrm.Blazor.Components;
using SalesCrm.Blazor.Services;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
// Use your Syncfusion license key from Environment variable.
string? licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
SyncfusionLicenseProvider.RegisterLicense(licenseKey);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSyncfusionBlazor();

// HttpClient for the Sales CRM Web API.
builder.Services.AddHttpClient<ISalesCrmApiClient, SalesCrmApiClient>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var environment = sp.GetRequiredService<IHostEnvironment>();

    var baseAddress = configuration["SalesCrmApi:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseAddress))
    {
        if (environment.IsDevelopment())
        {
            baseAddress = "https://localhost:59527/";
        }
        else
        {
            throw new InvalidOperationException(
                "Required configuration 'SalesCrmApi:BaseUrl' is missing.");
        }
    }

    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<ThemeService>();

// AI chat: register the local rule-based responder as the default IChatClient.
// To use a real LLM later, replace the LocalSalesChatClient registration with a
// real IChatClient (OpenAI/Azure/Ollama) while keeping the same service type.
builder.Services.AddSingleton<LocalSalesChatClient>();
builder.Services.AddScoped(sp => (IChatClient)sp.GetRequiredService<LocalSalesChatClient>());

builder.Services.AddLocalization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
