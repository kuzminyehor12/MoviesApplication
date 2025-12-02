using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IDatabase<MovieDbContext> _database;

    public MovieService(IDatabase<MovieDbContext> database)
    {
        _database = database;
    }

    public async Task<MovieExtendedModel> GetMovieExtendedAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var movie = await _database.Store<Movie>().FirstAsync(m => m.Id == movieId, cancellationToken: cancellationToken);
        return MovieExtendedModel.Create(movie);
    }
}