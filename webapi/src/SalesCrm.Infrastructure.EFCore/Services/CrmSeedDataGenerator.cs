using Microsoft.EntityFrameworkCore;
using SalesCrm.Infrastructure.Abstractions.Services;
using SalesCrm.Infrastructure.EFCore.Persistence;
using SalesCrm.SeedData;

namespace SalesCrm.Infrastructure.EFCore.Services;

public sealed class CrmSeedDataGenerator : ICrmSeedDataGenerator
{
    private readonly SalesCrmDbContext dbContext;
    private readonly DatabaseSeeder databaseSeeder;

    public CrmSeedDataGenerator(
        SalesCrmDbContext dbContext,
        DatabaseSeeder databaseSeeder)
    {
        this.dbContext = dbContext;
        this.databaseSeeder = databaseSeeder;
    }

    public async Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        var seedData = databaseSeeder.Generate();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE
                "ai_recommendations",
                "activities",
                "forecasts",
                "sales_documents",
                "quotes",
                "opportunities",
                "contacts",
                "accounts",
                "document_templates",
                "territories",
                "users"
            RESTART IDENTITY CASCADE;
            """, cancellationToken);

        dbContext.ChangeTracker.Clear();

        dbContext.Users.AddRange(seedData.Users);
        dbContext.Territories.AddRange(seedData.Territories);
        dbContext.DocumentTemplates.AddRange(seedData.DocumentTemplates);
        await dbContext.SaveChangesAsync(cancellationToken);

        // 1. Save parent table first.
        dbContext.Accounts.AddRange(seedData.Accounts);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Optional safety check before saving contacts.
        var savedAccountIds = await dbContext.Accounts
            .AsNoTracking()
            .Select(account => account.Id)
            .ToHashSetAsync(cancellationToken);

        var invalidContacts = seedData.Contacts
            .Where(contact => !savedAccountIds.Contains(contact.AccountId))
            .Select(contact => new { contact.Id, contact.AccountId })
            .Take(10)
            .ToList();

        if (invalidContacts.Count > 0)
        {
            var details = string.Join(", ", invalidContacts.Select(x => $"{x.Id} -> {x.AccountId}"));
            throw new InvalidOperationException($"Invalid contact AccountId values found: {details}");
        }

        // 2. Save contacts only after accounts exist in PostgreSQL.
        dbContext.Contacts.AddRange(seedData.Contacts);
        await dbContext.SaveChangesAsync(cancellationToken);

        // 3. Save opportunities only after accounts and contacts exist.
        dbContext.Opportunities.AddRange(seedData.Opportunities);
        await dbContext.SaveChangesAsync(cancellationToken);

        // 4. Save remaining child tables.
        dbContext.Activities.AddRange(seedData.Activities);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Forecasts.AddRange(seedData.Forecasts);
        dbContext.SalesDocuments.AddRange(seedData.SalesDocuments);
        dbContext.AiRecommendations.AddRange(seedData.AiRecommendations);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
