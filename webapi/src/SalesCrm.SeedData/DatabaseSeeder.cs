namespace SalesCrm.SeedData;

public sealed class DatabaseSeeder
{
    private readonly CrmSeedOptions _options;
    private readonly UserBogusGenerator _userGenerator;
    private readonly TerritoryBogusGenerator _territoryGenerator;
    private readonly DocumentTemplateBogusGenerator _documentTemplateGenerator;
    private readonly AccountBogusGenerator _accountGenerator;
    private readonly ContactBogusGenerator _contactGenerator;
    private readonly OpportunityBogusGenerator _opportunityGenerator;
    private readonly ActivityBogusGenerator _activityGenerator;
    private readonly ForecastBogusGenerator _forecastGenerator;
    private readonly DocumentBogusGenerator _documentGenerator;
    private readonly AiRecommendationBogusGenerator _recommendationGenerator;

    public DatabaseSeeder(CrmSeedOptions options)
    {
        _options = options;
        _userGenerator = new UserBogusGenerator();
        _territoryGenerator = new TerritoryBogusGenerator();
        _documentTemplateGenerator = new DocumentTemplateBogusGenerator();
        _accountGenerator = new AccountBogusGenerator();
        _contactGenerator = new ContactBogusGenerator();
        _opportunityGenerator = new OpportunityBogusGenerator();
        _activityGenerator = new ActivityBogusGenerator();
        _forecastGenerator = new ForecastBogusGenerator();
        _documentGenerator = new DocumentBogusGenerator();
        _recommendationGenerator = new AiRecommendationBogusGenerator();
    }

    public CrmSeedData Generate()
    {
        var users = _userGenerator.Generate(_options.UserCount);
        var territories = _territoryGenerator.Generate(users, _options.TerritoryCount);
        var documentTemplates = _documentTemplateGenerator.Generate(_options.DocumentTemplateCount);
        var accounts = _accountGenerator.Generate(_options.Seed, territories, users, _options.AccountCount);
        var contacts = _contactGenerator.Generate(_options.Seed + 1, accounts, _options.ContactCount);
        var opportunities = _opportunityGenerator.Generate(accounts, contacts, users, _options.OpportunityCount);
        var activities = _activityGenerator.Generate(accounts, contacts, opportunities, users, _options.ActivityCount);
        var forecasts = _forecastGenerator.Generate(opportunities, _options.ForecastCount);
        var documents = _documentGenerator.Generate(opportunities, documentTemplates, users, _options.DocumentCount);
        var recommendations = _recommendationGenerator.Generate(opportunities, _options.AiRecommendationCount);

        return new CrmSeedData(users, territories, documentTemplates, accounts, contacts, opportunities, activities, forecasts, documents, recommendations);
    }

    public Task GenerateAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = Generate();
        return Task.CompletedTask;
    }
}
