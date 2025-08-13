namespace Movies.Core.Entities;

public abstract class BaseEntity<TId> : IEntity
{
    public TId? Id { get; init; } = default(TId);
}