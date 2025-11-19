using Movies.Core.Entities;

namespace Movies.Application.Models;

public class MovieViewModel
{
    public decimal Score { get; init; }
    
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

    public static MovieViewModel Create(MovieScoreResult result)
    {
        return new MovieViewModel
        {
            Score = result.Score,
            Title = result.Movie.Title,
            Genres = result.Movie.Genres,
            Overview = result.Movie.Overview,
            Popularity = result.Movie.Popularity,
            ReleaseDate = result.Movie.ReleaseDate,
            TagLine = result.Movie.TagLine,
            CastMembers = result.Movie.CastMembers?.Take(5),
            CrewMembers = result.Movie.CrewMembers?.Take(3),
            Keywords = result.Movie.Keywords,
            VoteAverage = result.Movie.VoteAverage,
            VoteCount = result.Movie.VoteCount,
            PosterPath = result.Movie.PosterPath,
            ImdbId = result.Movie.ImdbId
        };
    }
    
    public static MovieViewModel Create(Movie movie)
    {
        return new MovieViewModel
        {
            Score = 0,
            Title = movie.Title,
            Genres = movie.Genres,
            Overview = movie.Overview,
            Popularity = movie.Popularity,
            ReleaseDate = movie.ReleaseDate,
            TagLine = movie.TagLine,
            CastMembers = movie.CastMembers?.Take(5),
            CrewMembers = movie.CrewMembers?.Take(3),
            Keywords = movie.Keywords,
            VoteAverage = movie.VoteAverage,
            VoteCount = movie.VoteCount,
            PosterPath = movie.PosterPath,
            ImdbId = movie.ImdbId
        };
    }
}