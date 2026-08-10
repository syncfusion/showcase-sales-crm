namespace SalesCrm.Domain.Common;

public abstract class EntityBase<TId>
    where TId : notnull
{
    protected EntityBase()
    {
        Id = default!;
    }

    protected EntityBase(TId id)
    {
        Id = id;
    }

    public TId Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return GetType() == other.GetType()
            && EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(EntityBase<TId>? left, EntityBase<TId>? right)
        => EqualityComparer<EntityBase<TId>?>.Default.Equals(left, right);

    public static bool operator !=(EntityBase<TId>? left, EntityBase<TId>? right)
        => !(left == right);
}
