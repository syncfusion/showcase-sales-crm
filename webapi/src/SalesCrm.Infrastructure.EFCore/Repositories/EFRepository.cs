using Microsoft.EntityFrameworkCore;
using SalesCrm.Infrastructure.Abstractions.Repositories;
using SalesCrm.Infrastructure.EFCore.Persistence;

namespace SalesCrm.Infrastructure.EFCore.Repositories;

public class EFRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    protected readonly SalesCrmDbContext DbContext;

    protected readonly DbSet<TEntity> DbSet;

    public EFRepository(SalesCrmDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public virtual Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => DbSet.FindAsync([id], cancellationToken).AsTask();

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual void Update(TEntity entity) => DbSet.Update(entity);

    public virtual void Remove(TEntity entity) => DbSet.Remove(entity);
}
