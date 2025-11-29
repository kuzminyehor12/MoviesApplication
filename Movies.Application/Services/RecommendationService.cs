using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Movies.Application.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IDatabase<MovieDbContext> _database;

    public RecommendationService(IDatabase<MovieDbContext> database)
    {
        _database = database;
    }

    public async Task<IEnumerable<MovieViewModel>> GetRecommendationsAsync(int movieId, CancellationToken cancellationToken)
    {
        const int recommendationsCount = 5;
        
        var movie = await _database.Store<Movie>()
            .AsQueryable()
            .Include(m => m.Embedding)
            .FirstAsync(m => m.Id == movieId, cancellationToken: cancellationToken);

        var pgVector = new Vector(movie.Embedding.Vector);
        
        var movies = await _database.Store<Movie>()
            .AsQueryable()
            .Where(m => m.Id != movie.Id)
            .Include(m => m.Embedding)
            .OrderBy(m => m.Embedding.Vector.CosineDistance(pgVector))
            .Take(recommendationsCount)
            .ToListAsync(cancellationToken);
        
        var viewModels = movies.Select(MovieViewModel.Create);
        return viewModels;
    }
}