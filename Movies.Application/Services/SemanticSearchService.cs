using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using Pgvector.EntityFrameworkCore;

namespace Movies.Application.Services;

public class SemanticSearchService : ISemanticSearchService
{
    private readonly IDatabase<MovieDbContext> _database;
    private readonly IVectorGenerator _vectorGenerator;

    public SemanticSearchService(IDatabase<MovieDbContext> database, IVectorGenerator vectorGenerator)
    {
        _database = database;
        _vectorGenerator = vectorGenerator;
    }

    public async Task<PaginatedResult<MovieViewModel>> SearchAsync(SemanticSearchRequest request, CancellationToken cancellationToken)
    {
        var vector = await _vectorGenerator.VectorizeAsync(request.Query, cancellationToken);
        
        var movies = await _database.Store<Movie>()
            .AsQueryable()
            .Include(m => m.Embedding)
            .OrderBy(m => m.Embedding.Vector.CosineDistance(vector))
            .ToListAsync(cancellationToken);
        
        var viewModels = movies.Select(MovieViewModel.Create);
        return PaginatedResult<MovieViewModel>.Create(viewModels, request.PageNumber);
    }
}