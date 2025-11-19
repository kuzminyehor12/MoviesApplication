using System.Text;

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
    
    public ICollection<TermFieldMap>? FieldMaps { get; init; }

    public override string ToString()
    {
        var text = new StringBuilder();

        text.AppendLine(Title);

        if (Genres != null)
        {
            text.AppendLine(string.Join(',', Genres.Select(g => "[" + g.Name + "]")));
        }

        if (Overview != null)
        {
            text.AppendLine(Overview);
        }
        
        if (TagLine != null)
        {
            text.AppendLine(TagLine);
        }
        
        if (Keywords != null)
        {
            text.AppendLine(string.Join(',', Keywords.Select(g => "[" + g.Name + "]")));
        }
        
        if (CastMembers != null)
        {
            text.AppendLine(string.Join(',', CastMembers.Take(5).Select(g => "[" + g.Name + "]")));
            text.AppendLine(string.Join(',', CastMembers.Where(cast => !string.IsNullOrWhiteSpace(cast.Character)).Select(cast => "[" + cast.Character + "]").Take(5)));
        }
        
        if (CrewMembers != null)
        {
            text.AppendLine(string.Join(',', CrewMembers.Take(3).Select(g => "[" + g.Name + "]")));
        }

        return text.ToString();
    }
}