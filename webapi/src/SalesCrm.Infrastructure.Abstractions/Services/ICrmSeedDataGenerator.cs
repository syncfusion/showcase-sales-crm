namespace SalesCrm.Infrastructure.Abstractions.Services;

public interface ICrmSeedDataGenerator
{
    Task GenerateAsync(CancellationToken cancellationToken = default);
}
