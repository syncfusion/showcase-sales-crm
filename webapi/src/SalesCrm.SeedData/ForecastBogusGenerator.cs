//using SalesCrm.Domain.Entities;
//using SalesCrm.Domain.ValueObjects;

//namespace SalesCrm.SeedData;

//public sealed class ForecastBogusGenerator
//{
//    public IReadOnlyList<Forecast> Generate(IReadOnlyList<Opportunity> opportunities, int count)
//    {
//        if (opportunities.Count == 0)
//        {
//            throw new InvalidOperationException("Cannot generate forecasts without opportunities.");
//        }

//        if (count > DeterministicIds.Forecasts.Length)
//        {
//            throw new InvalidOperationException($"Forecast count {count} exceeds deterministic ID count {DeterministicIds.Forecasts.Length}.");
//        }

//        var items = new List<Forecast>(count);

//        for (var i = 0; i < count; i++)
//        {
//            var opportunity = opportunities[i % opportunities.Count];
//            var relatedOpportunity = opportunities[(i + 1) % opportunities.Count];

//            items.Add(new Forecast(
//                DeterministicIds.Forecasts[i],
//                opportunity.AccountId,
//                relatedOpportunity.AccountId,
//                new[] { "Commit", "Best Case", "Pipeline", "Omitted" }[i % 4],
//                new Money(opportunity.EstimatedValue.Amount * 0.75m, opportunity.EstimatedValue.Currency),
//                new Money(opportunity.EstimatedValue.Amount * 0.85m, opportunity.EstimatedValue.Currency),
//                new Money(opportunity.EstimatedValue.Amount, opportunity.EstimatedValue.Currency),
//                DateOnly.FromDateTime(new DateTime(2026, 1, 1).AddMonths(i % 6)).ToString("yyyy-MM")));
//        }

//        return items;
//    }
//}

//using SalesCrm.Domain.Entities;
//using SalesCrm.Domain.ValueObjects;

//namespace SalesCrm.SeedData;

//public sealed class ForecastBogusGenerator
//{
//    private static readonly string[] ForecastCategories =
//    [
//        "Commit",
//        "Best Case",
//        "Pipeline",
//        "Omitted"
//    ];

//    public IReadOnlyList<Forecast> Generate(IReadOnlyList<Opportunity> opportunities, int count)
//    {
//        if (opportunities.Count == 0)
//        {
//            throw new InvalidOperationException("Cannot generate forecasts without opportunities.");
//        }

//        if (count > DeterministicIds.Forecasts.Length)
//        {
//            throw new InvalidOperationException(
//                $"Forecast count {count} exceeds deterministic ID count {DeterministicIds.Forecasts.Length}.");
//        }

//        var forecastMonths = GetForecastMonths();
//        var items = new List<Forecast>(count);

//        for (var i = 0; i < count; i++)
//        {
//            var opportunity = opportunities[i % opportunities.Count];
//            var relatedOpportunity = opportunities[(i + 1) % opportunities.Count];
//            var forecastMonth = forecastMonths[i % forecastMonths.Count];

//            var periodFactor = GetPeriodFactor(forecastMonth);
//            var estimatedAmount = opportunity.EstimatedValue.Amount;

//            items.Add(new Forecast(
//                DeterministicIds.Forecasts[i],
//                opportunity.AccountId,
//                relatedOpportunity.AccountId,
//                ForecastCategories[i % ForecastCategories.Length],
//                new Money(estimatedAmount * 0.75m * periodFactor, opportunity.EstimatedValue.Currency),
//                new Money(estimatedAmount * 0.85m * periodFactor, opportunity.EstimatedValue.Currency),
//                new Money(estimatedAmount * periodFactor, opportunity.EstimatedValue.Currency),
//                forecastMonth.ToString("MM-yyyy")));
//        }

//        return items;
//    }

//    private static IReadOnlyList<DateOnly> GetForecastMonths()
//    {
//        var today = DateTime.Today;
//        var currentMonth = new DateOnly(today.Year, today.Month, 1);

//        var months = new List<DateOnly>(10);

//        // Past 6 completed months + current month + future 3 months.
//        for (var monthOffset = -6; monthOffset <= 3; monthOffset++)
//        {
//            months.Add(currentMonth.AddMonths(monthOffset));
//        }

//        return months;
//    }

//    private static decimal GetPeriodFactor(DateOnly forecastMonth)
//    {
//        var today = DateTime.Today;
//        var currentMonth = new DateOnly(today.Year, today.Month, 1);

//        var monthDifference =
//            ((forecastMonth.Year - currentMonth.Year) * 12) +
//            forecastMonth.Month -
//            currentMonth.Month;

//        return monthDifference switch
//        {
//            < 0 => 0.90m, // Past months.
//            0 => 0.95m,  // Current month prediction.
//            1 => 1.00m,  // Next month forecast.
//            2 => 1.08m,  // Future growth forecast.
//            3 => 1.15m,  // Longer future pipeline forecast.
//            _ => 1.00m
//        };
//    }
//}
using SalesCrm.Domain.Entities;
using SalesCrm.Domain.ValueObjects;

namespace SalesCrm.SeedData;

public sealed class ForecastBogusGenerator
{
    private static readonly string[] ForecastCategories =
    [
        "Commit",
        "Best Case",
        "Pipeline",
        "Omitted"
    ];

    public IReadOnlyList<Forecast> Generate(IReadOnlyList<Opportunity> opportunities, int count)
    {
        if (opportunities.Count == 0)
        {
            throw new InvalidOperationException("Cannot generate forecasts without opportunities.");
        }

        if (count > DeterministicIds.Forecasts.Length)
        {
            throw new InvalidOperationException(
                $"Forecast count {count} exceeds deterministic ID count {DeterministicIds.Forecasts.Length}.");
        }

        var forecastMonths = GetForecastMonths();
        var monthlyForecastValues = GenerateMonthlyForecastValues(opportunities, forecastMonths);

        var items = new List<Forecast>(count);

        for (var i = 0; i < count; i++)
        {
            var opportunity = opportunities[i % opportunities.Count];
            var relatedOpportunity = opportunities[(i + 1) % opportunities.Count];
            var forecastMonth = forecastMonths[i % forecastMonths.Count];

            var weightedForecastAmount = monthlyForecastValues[forecastMonth];

            items.Add(new Forecast(
                DeterministicIds.Forecasts[i],
                opportunity.AccountId,
                relatedOpportunity.AccountId,
                ForecastCategories[i % ForecastCategories.Length],
                new Money(weightedForecastAmount * 0.90m, opportunity.EstimatedValue.Currency),
                new Money(weightedForecastAmount, opportunity.EstimatedValue.Currency),
                new Money(weightedForecastAmount * 1.15m, opportunity.EstimatedValue.Currency),
                forecastMonth.ToString("yyyy-MM")));
        }

        return items;
    }

    private static IReadOnlyList<DateOnly> GetForecastMonths()
    {
        var today = DateTime.Today;
        var currentMonth = new DateOnly(today.Year, today.Month, 1);

        var months = new List<DateOnly>(10);

        // Past 6 months + current month + future 3 months.
        for (var monthOffset = -6; monthOffset <= 3; monthOffset++)
        {
            months.Add(currentMonth.AddMonths(monthOffset));
        }

        return months;
    }

    private static Dictionary<DateOnly, decimal> GenerateMonthlyForecastValues(
        IReadOnlyList<Opportunity> opportunities,
        IReadOnlyList<DateOnly> forecastMonths)
    {
        var today = DateTime.Today;
        var currentMonth = new DateOnly(today.Year, today.Month, 1);

        var averageOpportunityValue = opportunities.Average(
            opportunity => opportunity.EstimatedValue.Amount);

        // Start slightly below the average and grow month by month.
        var previousMonthValue = averageOpportunityValue * 1.02m;

        var values = new Dictionary<DateOnly, decimal>();

        foreach (var forecastMonth in forecastMonths)
        {
            var monthDifference =
                ((forecastMonth.Year - currentMonth.Year) * 12) +
                forecastMonth.Month -
                currentMonth.Month;

            var growthFactor = monthDifference switch
            {
                < 0 => 1.04m, // Past trend steadily improves.
                0 => 1.03m,  // Current month should continue from previous month.
                1 => 1.0m,  // Near future forecast.
                2 => 1.05m,  // Future expected growth.
                3 => 1.05m,  // Further future forecast.
                _ => 1.00m
            };

            var currentMonthValue = previousMonthValue * growthFactor;

            values[forecastMonth] = Math.Round(currentMonthValue, 2);

            previousMonthValue = currentMonthValue;
        }

        return values;
    }
}