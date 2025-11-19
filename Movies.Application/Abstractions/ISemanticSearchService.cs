using Movies.Application.Models;
using Movies.Application.Requests;

namespace Movies.Application.Abstractions;

public interface ISemanticSearchService
{
    Task<PaginatedResult<MovieViewModel>> SearchAsync(SemanticSearchRequest request, CancellationToken cancellationToken);
}