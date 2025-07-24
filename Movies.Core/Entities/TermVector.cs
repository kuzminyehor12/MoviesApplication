using Movies.Core.Enums;

namespace Movies.Core.Entities;

public class TermVector
{
    public required int MovieId { get; init; }
    
    public required int TermId { get; init; }
    
    public required FieldType FieldType { get; init; }
    
    public double Score { get; init; }
    
    public Movie Movie { get; init; } = null!;
    
    public Term Term { get; init; } = null!;
}