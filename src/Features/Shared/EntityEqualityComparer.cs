namespace MagicMatchTracker.Features.Shared;

public sealed class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : IEntity
{
	private readonly EqualityComparer<Guid> _guidComparer = EqualityComparer<Guid>.Default;
	private readonly EqualityComparer<TEntity> _entityComparer = EqualityComparer<TEntity>.Default;

	private EntityEqualityComparer() { }

	public static EntityEqualityComparer<TEntity> Default { get; } = new();

	public bool Equals(TEntity? x, TEntity? y)
	{
		if (x is null && y is null)
			return true;
		if (x is null || y is null)
			return false;
		if (x.Id == Guid.Empty && y.Id == Guid.Empty)
			return _entityComparer.Equals(x, y);

		return _guidComparer.Equals(x.Id, y.Id);
	}

	public int GetHashCode(TEntity obj)
	{
		if (obj.Id == Guid.Empty)
			return _entityComparer.GetHashCode(obj);

		return _guidComparer.GetHashCode(obj.Id);
	}
}