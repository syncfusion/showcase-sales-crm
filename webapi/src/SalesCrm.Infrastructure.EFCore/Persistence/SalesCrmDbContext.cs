using Microsoft.EntityFrameworkCore;
using SalesCrm.Domain.Entities;

namespace SalesCrm.Infrastructure.EFCore.Persistence;

public sealed class SalesCrmDbContext : DbContext
{
    public SalesCrmDbContext(DbContextOptions<SalesCrmDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<PipelineStage> PipelineStages => Set<PipelineStage>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Territory> Territories => Set<Territory>();
    public DbSet<Forecast> Forecasts => Set<Forecast>();
    public DbSet<SalesDocument> SalesDocuments => Set<SalesDocument>();
    public DbSet<DocumentTemplate> DocumentTemplates => Set<DocumentTemplate>();
    public DbSet<AiRecommendation> AiRecommendations => Set<AiRecommendation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesCrmDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
