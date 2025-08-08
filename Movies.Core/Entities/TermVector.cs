using Movies.Core.Enums;

namespace Movies.Core.Entities;

public class TermVector : BaseEntity<(int MovieId, int TermId)>
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
    
    public required FieldType FieldType { get; init; }
    
    public double Score { get; init; }
    
    public Movie Movie { get; init; } = null!;
    
    public Term Term { get; init; } = null!;
}