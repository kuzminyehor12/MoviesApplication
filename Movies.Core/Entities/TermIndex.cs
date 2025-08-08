namespace Movies.Core.Entities;

public class TermIndex : BaseEntity<(int TermId, int MovieId)>
{
    public int TermId
    {
        get => Id.TermId;
        init => Id = (value, MovieId);
    }
    
    public int MovieId
    {
        get => Id.MovieId;
        init => Id = (TermId, value);
    }
    
    public float TermFrequency { get; init; }

    public int[] TermPositions { get; init; } = [];

    public Term Term { get; init; } = null!;
    
    public Movie Movie { get; init; } = null!;
}