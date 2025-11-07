using Movies.Core.Entities;

namespace Movies.Application.Abstractions;

public interface ISemanticSearchService
{
    Task<IEnumerable<Movie>> SearchAsync(string query, CancellationToken cancellationToken);
}