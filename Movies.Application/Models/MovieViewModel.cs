using Movies.Core.Entities;

namespace Movies.Application.Models;

public class MovieViewModel
{
    public int Id { get; init; }
    
    public required string Title { get; init; }

    public IEnumerable<Genre>? Genres { get; init; } = [];

    public DateTime? ReleaseDate { get; init; }
    
    public float VoteAverage { get; init; }
    
    public string? PosterPath { get; init; }
    
    public string? ImdbId { get; init; }

    public static MovieViewModel Create(MovieScoreResult result)
    {
        return new MovieViewModel
        {
            Id = result.Movie.Id,
            Title = result.Movie.Title,
            Genres = result.Movie.Genres,
            ReleaseDate = result.Movie.ReleaseDate,
            VoteAverage = result.Movie.VoteAverage,
            PosterPath = result.Movie.PosterPath,
            ImdbId = result.Movie.ImdbId
        };
    }
    
    public static MovieViewModel Create(Movie movie)
    {
        return new MovieViewModel
        {
            Id = movie.Id,
            Title = movie.Title,
            Genres = movie.Genres,
            ReleaseDate = movie.ReleaseDate,
            VoteAverage = movie.VoteAverage,
            PosterPath = movie.PosterPath,
            ImdbId = movie.ImdbId
        };
    }
}