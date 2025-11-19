using Movies.Core.Enums;

namespace Movies.Search.Models;

public class Token
{
    public required string Term { get; set; }
    
    public required TermType TermType { get; init; }
    
    public required FieldType FieldType { get; init; }
    
    public float Frequency { get; init; }

    public int[] Positions { get; init; } = [];
}