namespace Movies.Core.Entities;

public class CastMember : BaseEntity<int>
{
    public string? Character { get; init; }
    
    public required string Name { get; init; }
    
    public string? ProfilePath { get; init; }
}