using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.Application.Services;

public class SemanticSearchService : ISemanticSearchService
{
    private readonly IDatabase<MovieDbContext> _database;
    private readonly ITokenExtractor _tokenExtractor;

    public SemanticSearchService(IDatabase<MovieDbContext> database, ITokenExtractor tokenExtractor)
    {
        _database = database;
        _tokenExtractor = tokenExtractor;
    }

    public async Task<PaginatedResult<MovieViewModel>> SearchAsync(SemanticSearchRequest request, CancellationToken cancellationToken)
    {
        var tokens = _tokenExtractor.Extract(request.Query);

        var termArray = tokens.Select(t => t.Term).ToArray();
        
        var matchingTermIds = _database.Store<Term>()
            .AsQueryable()
            .Where(t => termArray.Contains(t.TermText) && t.TermType == TermType.WholeWord)
            .Select(t => t.Id);
        
        const decimal highWeight = 1.5m;
        const decimal maxWeight = 2m;
        const decimal lowWeight = 0.5m;
        const decimal defaultWeight = 1.0m;
        
       /* var aggregatedScores = _database.Store<TermVector>()
            .AsQueryable()
            .Where(vector => matchingTermIds.Contains(vector.TermId))
            .GroupBy(vector => vector.MovieId)
            .Select(g => new 
            {
                MovieId = g.Key,
                TotalScore = g.Sum(vector => (decimal)vector.Score * (
                    vector.Term.FieldType == FieldType.Title ? maxWeight :
                        vector.Term.FieldType == FieldType.Overview ? lowWeight :
                            vector.Term.FieldType == FieldType.TagLine ? lowWeight :
                                vector.Term.FieldType == FieldType.Genre ||
                                vector.Term.FieldType == FieldType.Keyword ||
                                vector.Term.FieldType == FieldType.Character ||
                                vector.Term.FieldType == FieldType.Actor ||
                                vector.Term.FieldType == FieldType.Crew 
                                    ? highWeight 
                                    : defaultWeight
                )) 
            })
            .Where(result => result.TotalScore > 0.0m);
        
        var movies = await _database.Store<Movie>()
            .AsQueryable()
            .Join(
                aggregatedScores, 
                movie => movie.Id, 
                agg => agg.MovieId,
                (movie, agg) => new MovieScoreResult 
                {
                    Movie = movie,
                    Score = agg.TotalScore
                })
            .OrderByDescending(result => result.Score)
            .ToListAsync(cancellationToken);

        var viewModels = movies.Select(MovieViewModel.Create);
        return PaginatedResult<MovieViewModel>.Create(viewModels, request.PageNumber);*/
       throw new NotImplementedException();
    }
}