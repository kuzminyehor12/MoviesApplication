using Movies.Application.Models;
using Movies.Application.Requests;

namespace Movies.Application.Abstractions;

public interface ILexicalSearchService
{
    Task<PaginatedResult<MovieViewModel>> SearchAsync(LexicalSearchRequest request, CancellationToken cancellationToken);
}