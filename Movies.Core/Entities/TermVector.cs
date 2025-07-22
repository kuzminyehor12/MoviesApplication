namespace Movies.Core.Entities;

public class TermVector
{
    public required long MovieId { get; init; }
    
    public required long TermId { get; init; }
    
    public double Score { get; init; }
    
    public Movie Movie { get; init; } = null!;
    
    public Term Term { get; init; } = null!;
}