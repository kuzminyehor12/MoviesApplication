namespace Movies.Core.Entities;

public class Genre : BaseEntity<int>
{
    public required string Name { get; init; }
}