using Movies.Core.Entities;

namespace Movies.Application.Abstractions;

public interface IRecommendationService
{
    Task<IEnumerable<Movie>> GetRecommendationsAsync(int movieId, CancellationToken cancellationToken);
}