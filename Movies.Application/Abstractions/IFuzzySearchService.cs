using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;

namespace Movies.Application.Abstractions;

public interface IFuzzySearchService
{
    Task<PaginatedResult<MovieViewModel>> SearchAsync(FuzzySearchRequest request, CancellationToken cancellationToken);
}