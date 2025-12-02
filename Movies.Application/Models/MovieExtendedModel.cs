using Movies.Core.Entities;

namespace Movies.Application.Models;

public class MovieExtendedModel
{
    public int Id { get; init; }
    
    public required string Title { get; init; }

    public IEnumerable<Genre>? Genres { get; init; } = [];
    
    public string? Overview { get; init; }
    
    public double Popularity { get; init; }

    public DateTime? ReleaseDate { get; init; }

    public string? TagLine { get; init; }

    public IEnumerable<CastMember>? CastMembers { get; init; } = [];

    public IEnumerable<CrewMember>? CrewMembers { get; init; } = [];

    public IEnumerable<Keyword>? Keywords { get; init; } = [];
    
    public float VoteAverage { get; init; }
    
    public int VoteCount { get; init; }
    
    public string? PosterPath { get; init; }
    
    public string? ImdbId { get; init; }

    public static MovieExtendedModel Create(Movie movie)
    {
        return new MovieExtendedModel
        {
            Id = movie.Id,
            Title = movie.Title,
            Genres = movie.Genres,
            Overview = movie.Overview,
            Popularity = movie.Popularity,
            ReleaseDate = movie.ReleaseDate,
            TagLine = movie.TagLine,
            CastMembers = movie.CastMembers,
            CrewMembers = movie.CrewMembers,
            Keywords = movie.Keywords,
            VoteAverage = movie.VoteAverage,
            VoteCount = movie.VoteCount,
            PosterPath = movie.PosterPath,
            ImdbId = movie.ImdbId
        };
    }
}