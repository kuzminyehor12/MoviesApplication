namespace Movies.Core.Entities;

public class Movie : BaseEntity<int>
{
    public required string Title { get; init; }

    public Genre[]? Genres { get; init; } = [];
    
    public string? Overview { get; init; }
    
    public double Popularity { get; init; }

    public DateTime? ReleaseDate { get; init; }

    public string? TagLine { get; init; }

    public CastMember[]? CastMembers { get; init; } = [];

    public CrewMember[]? CrewMembers { get; init; } = [];

    public Keyword[]? Keywords { get; init; } = [];
    
    public float VoteAverage { get; init; }
    
    public int VoteCount { get; init; }
    
    public string? PosterPath { get; init; }
    
    public string? ImdbId { get; init; }
    
    public ICollection<TermIndex>? TermIndex { get; init; }
    
    public ICollection<TermVector>? TermVectors { get; init; }
}