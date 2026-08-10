namespace SalesCrm.Infrastructure.Abstractions.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
