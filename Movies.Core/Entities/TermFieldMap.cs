using Movies.Core.Enums;

namespace Movies.Core.Entities;

public class TermFieldMap: BaseEntity<int>
{
    public int MovieId { get; init; }
   
    public int TermId { get; init; }
    
    public required FieldType FieldType { get; init; } = FieldType.None;

    public Movie Movie { get; init; } = null!;

    public Term Term { get; init; } = null!;
}