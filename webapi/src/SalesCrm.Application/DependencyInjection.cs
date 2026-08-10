using Microsoft.Extensions.DependencyInjection;

namespace SalesCrm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesCrmApplication(this IServiceCollection services)
    {
        services.AddScoped<Services.IDashboardService, Services.DashboardService>();
        services.AddScoped<Services.IAccountService, Services.AccountService>();
        services.AddScoped<Services.IContactService, Services.ContactService>();
        services.AddScoped<Services.IOpportunityService, Services.OpportunityService>();
        services.AddScoped<Services.IPipelineService, Services.PipelineService>();
        services.AddScoped<Services.IForecastService, Services.ForecastService>();
        services.AddScoped<Services.IActivityService, Services.ActivityService>();
        services.AddScoped<Services.ITerritoryService, Services.TerritoryService>();
        services.AddScoped<Services.IDocumentService, Services.DocumentService>();
        services.AddScoped<Services.IAiRecommendationService, Services.AiRecommendationService>();
        services.AddScoped<Services.IAdminSeedService, Services.AdminSeedService>();
        return services;
    }
}
