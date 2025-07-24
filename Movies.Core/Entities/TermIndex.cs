namespace Movies.Core.Entities;

public class TermIndex
{
    public required int TermId { get; init; }
    public required int MovieId { get; init; }
    
    public float TermFrequency { get; init; }

    public int[] TermPositions { get; init; } = [];

    public Term Term { get; init; } = null!;
    
    public Movie Movie { get; init; } = null!;
}