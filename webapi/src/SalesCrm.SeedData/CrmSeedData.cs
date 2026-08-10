using SalesCrm.Domain.Entities;

namespace SalesCrm.SeedData;

public sealed record CrmSeedData(
    IReadOnlyList<User> Users,
    IReadOnlyList<Territory> Territories,
    IReadOnlyList<DocumentTemplate> DocumentTemplates,
    IReadOnlyList<Account> Accounts,
    IReadOnlyList<Contact> Contacts,
    IReadOnlyList<Opportunity> Opportunities,
    IReadOnlyList<Activity> Activities,
    IReadOnlyList<Forecast> Forecasts,
    IReadOnlyList<SalesDocument> SalesDocuments,
    IReadOnlyList<AiRecommendation> AiRecommendations);
