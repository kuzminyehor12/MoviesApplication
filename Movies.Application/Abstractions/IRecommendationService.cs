using Movies.Application.Models;

namespace Movies.Application.Abstractions;

public interface IRecommendationService
{
    Task<IEnumerable<MovieViewModel>> GetRecommendationsAsync(int movieId, CancellationToken cancellationToken);
}