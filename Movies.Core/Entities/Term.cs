using Movies.Core.Enums;

namespace Movies.Core.Entities;

public class Term : BaseEntity<int>
{
    public required string TermText { get; init; }

    public required TermType TermType { get; init; } = TermType.WholeWord;
    
    public ICollection<TermIndex>? Index { get; init; }
    
    public ICollection<TermVector>? Vectors { get; init; }
}