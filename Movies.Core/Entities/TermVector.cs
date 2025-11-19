using Movies.Core.Enums;

namespace Movies.Core.Entities;

public class TermVector : BaseEntity<(int TermId, int MovieId)>
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
    
    public double Score { get; set; }
    
    public Movie Movie { get; init; } = null!;
    
    public Term Term { get; init; } = null!;
}