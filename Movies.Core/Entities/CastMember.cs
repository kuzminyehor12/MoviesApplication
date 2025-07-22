namespace Movies.Core.Entities;

public class CastMember : BaseEntity
{
    public string? Character { get; init; }
    
    public required string Name { get; init; }
    
    public string? ProfilePath { get; init; }
}