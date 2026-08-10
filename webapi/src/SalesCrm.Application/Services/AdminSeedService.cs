using SalesCrm.Contracts.Admin;
using SalesCrm.Infrastructure.Abstractions.Services;

namespace SalesCrm.Application.Services;

public interface IAdminSeedService
{
    Task<SeedRefreshResponse> RefreshAsync(CancellationToken cancellationToken = default);
}

public sealed class AdminSeedService : IAdminSeedService
{
    private readonly ICrmSeedDataGenerator seedDataGenerator;

    public AdminSeedService(ICrmSeedDataGenerator seedDataGenerator)
    {
        this.seedDataGenerator = seedDataGenerator;
    }

    public async Task<SeedRefreshResponse> RefreshAsync(CancellationToken cancellationToken = default)
    {
        await seedDataGenerator.GenerateAsync(cancellationToken);

        return new SeedRefreshResponse("Refreshed");
    }
}
