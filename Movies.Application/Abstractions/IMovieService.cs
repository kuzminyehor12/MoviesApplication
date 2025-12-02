using Movies.Application.Models;

namespace Movies.Application.Abstractions;

public interface IMovieService
{
    Task<MovieExtendedModel> GetMovieExtendedAsync(int movieId, CancellationToken cancellationToken = default);
}