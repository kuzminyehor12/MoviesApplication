using Movies.Core.Enums;

namespace Movies.Core.Entities;

public class Term : BaseEntity
{
    public required FieldType FieldType { get; init; }
    
    public required string TermText { get; init; }
    
    public ICollection<TermIndex>? Index { get; init; }
    
    public ICollection<TermVector>? Vectors { get; init; }
}