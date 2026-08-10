using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesCrm.Infrastructure.Abstractions.Repositories;
using SalesCrm.Infrastructure.Abstractions.Services;
using SalesCrm.Infrastructure.EFCore.Persistence;
using SalesCrm.Infrastructure.EFCore.Repositories;
using SalesCrm.Infrastructure.EFCore.Services;
using SalesCrm.SeedData;

namespace SalesCrm.Infrastructure.EFCore;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesCrmInfrastructureEfCore(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SalesCrmDatabase");
        services.AddDbContext<SalesCrmDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<,>), typeof(EFRepository<,>));
        services.AddScoped(typeof(IReadRepository<,>), typeof(EFReadRepository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton(new CrmSeedOptions());
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<ICrmSeedDataGenerator, CrmSeedDataGenerator>();

        return services;
    }
}
