using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using Pgvector;
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

        var pgVector = new Vector(vector);
        
        var movies = await _database.Store<Movie>()
            .AsQueryable()
            .Include(m => m.Embedding)
            .Select(m => new
            {
                Model = MovieViewModel.Create(m),
                Vector = m.Embedding.Vector,
            })
            .OrderBy(m => m.Vector.CosineDistance(pgVector))
            .Select(m => m.Model)
            .ToListAsync(cancellationToken);
        
        return PaginatedResult<MovieViewModel>.Create(movies, request.PageNumber);
    }
}