namespace SalesCrm.SeedData;

public static class DeterministicIds
{
    public static readonly Guid[] Users = CreateSeries(900);
    public static readonly Guid[] Territories = CreateSeries(800);
    public static readonly Guid[] DocumentTemplates = CreateSeries(700);
    public static readonly Guid[] Accounts = CreateSeries(1);
    public static readonly Guid[] Contacts = CreateSeries(100);
    public static readonly Guid[] Opportunities = CreateSeries(200);
    public static readonly Guid[] Activities = CreateSeries(300);
    public static readonly Guid[] Forecasts = CreateSeries(400);
    public static readonly Guid[] SalesDocuments = CreateSeries(500);
    public static readonly Guid[] AiRecommendations = CreateSeries(600);

    private static Guid[] CreateSeries(int offset)
    {
        var values = new Guid[64];
        for (var i = 0; i < values.Length; i++)
        {
            values[i] = new Guid($"{offset + i:D8}-0000-0000-0000-000000000000");
        }

        return values;
    }
}
