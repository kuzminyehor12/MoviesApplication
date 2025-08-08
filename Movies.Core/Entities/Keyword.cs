namespace Movies.Core.Entities;

public class Keyword : BaseEntity<int>
{
    public required string Name { get; init; }
}