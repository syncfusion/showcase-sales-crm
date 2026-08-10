using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace SalesCrm.Infrastructure.EFCore.Persistence;

public sealed class SalesCrmDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SalesCrmDbContext>
{
    public SalesCrmDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SalesCrm.Api"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SalesCrmDbContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("SalesCrmDb")
            ?? configuration.GetConnectionString("SalesCrmDatabase")
            ?? "Host=localhost;Port=5432;Database=salescrm_pipeline;Username=postgres;Password=postgres",
            npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(SalesCrmDbContext).Assembly.FullName));

        return new SalesCrmDbContext(optionsBuilder.Options);
    }
}