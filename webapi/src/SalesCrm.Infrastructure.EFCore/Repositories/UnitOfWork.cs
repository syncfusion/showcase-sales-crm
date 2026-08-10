using SalesCrm.Infrastructure.Abstractions.Repositories;
using SalesCrm.Infrastructure.EFCore.Persistence;

namespace SalesCrm.Infrastructure.EFCore.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SalesCrmDbContext _dbContext;

    public UnitOfWork(SalesCrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
