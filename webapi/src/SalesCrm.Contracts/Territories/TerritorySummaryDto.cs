namespace SalesCrm.Contracts.Territories;

public sealed record TerritorySummaryDto(string Id, string Name, string Region, decimal Revenue, int Accounts, int WinRate, double Latitude, double Longitude);
