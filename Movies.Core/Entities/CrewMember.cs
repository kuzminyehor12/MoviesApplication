namespace Movies.Core.Entities;

public class CrewMember : BaseEntity
{
    public required string Job { get; init; }
    
    public required string Name { get; init; }
    
    public string? ProfilePath { get; init; }
}