namespace Movies.Core.Entities;

public abstract class BaseEntity<TId>
{
    public TId? Id { get; init; } = default(TId);
}