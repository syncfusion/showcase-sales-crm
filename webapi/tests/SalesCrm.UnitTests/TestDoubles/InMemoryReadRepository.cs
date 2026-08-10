using SalesCrm.Infrastructure.Abstractions.Repositories;

namespace SalesCrm.UnitTests.TestDoubles;

internal sealed class InMemoryReadRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    private readonly IReadOnlyList<TEntity> items;
    private readonly Func<TEntity, TKey> keySelector;

    public InMemoryReadRepository(IReadOnlyList<TEntity> items, Func<TEntity, TKey> keySelector)
    {
        this.items = items;
        this.keySelector = keySelector;
    }

    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => Task.FromResult(items.FirstOrDefault(item => EqualityComparer<TKey>.Default.Equals(keySelector(item), id)));

    public Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity>? specification = null, CancellationToken cancellationToken = default)
    {
        var result = specification is null
            ? items
            : items.Where(specification.IsSatisfiedBy).ToList();

        return Task.FromResult(result);
    }

    public Task<IPagedResult<TEntity>> PagedAsync(ISpecification<TEntity>? specification = null, int page = 1, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var source = specification is null
            ? items
            : items.Where(specification.IsSatisfiedBy).ToList();
        var pagedItems = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult<IPagedResult<TEntity>>(new InMemoryPagedResult<TEntity>(pagedItems, page, pageSize, source.Count));
    }

    private sealed class InMemoryPagedResult<T> : IPagedResult<T>
    {
        public InMemoryPagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
    }
}
