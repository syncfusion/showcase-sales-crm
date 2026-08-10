using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SalesCrm.Application;
using SalesCrm.Application.Services;
using SalesCrm.Infrastructure.EFCore;
using SalesCrm.Infrastructure.EFCore.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSalesCrmApplication();
builder.Services.AddSalesCrmInfrastructureEfCore(builder.Configuration);

builder.Services.AddAuthentication("Bearer")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DemoAuthenticationHandler>("Bearer", _ => { });

builder.Services.AddAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
});

    // Configure CORS using origins from configuration (appsettings.json).
    // This allows changing allowed SPA origins at runtime (for example via Azure App Service settings).
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpaApps", policy =>
    {
        if (corsOrigins.Length == 0)
        {
            if (builder.Environment.IsDevelopment())
            {
                corsOrigins =
                [
                    "http://localhost:5173",
                    "http://127.0.0.1:5173",
                    "http://localhost:4200",
                    "http://127.0.0.1:4200"
                ];
            }
            else
            {
                throw new InvalidOperationException(
                    "Production CORS origins are not configured.");
            }
        }

        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<SalesCrmDbContext>();
    await db.Database.MigrateAsync();

    var seedService = scope.ServiceProvider.GetRequiredService<IAdminSeedService>();
    await seedService.RefreshAsync();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors("AllowSpaApps");
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404)
    {
        if (request.Path.StartsWithSegments("/api"))
        {
            response.ContentType = "application/json";
            await response.WriteAsJsonAsync(new
            {
                Status = 404,
                Message = "The requested API endpoint was not found."
            });
        }
        else
        {
            response.Redirect("/404.html");
        }
    }
});
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Timestamp = DateTime.UtcNow,
        Environment = app.Environment.EnvironmentName,
        Version = "1.0.0"
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales CRM Pipeline Intelligence API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Sales CRM Pipeline Intelligence API";
    });
}

app.MapOpenApi();
app.MapControllers();
app.Run();

public partial class Program;

static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddSingleton<ProblemDetailsFactory, DefaultProblemDetailsFactory>();
        return services;
    }
}

sealed class DemoAuthenticationHandler : Microsoft.AspNetCore.Authentication.AuthenticationHandler<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions>
{
    public DemoAuthenticationHandler(
        Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions> options,
        Microsoft.Extensions.Logging.ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader) || authHeader.Count == 0)
        {
            return Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "demo-user"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "demo.user@example.com"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "crm.read")
        };

        var identity = new System.Security.Claims.ClaimsIdentity(claims, Scheme.Name);
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var ticket = new Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(Microsoft.AspNetCore.Authentication.AuthenticateResult.Success(ticket));
    }
}
