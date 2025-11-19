using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Core.Extensions;
using Movies.Persistence.Batching;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.Application.Services;

public class FuzzySearchService : IFuzzySearchService
{
    private readonly IDatabase<MovieDbContext> _database;
    private readonly ITokenExtractor _tokenExtractor;

    public FuzzySearchService(
        IDatabase<MovieDbContext> database, 
        ITokenExtractor tokenExtractor)
    {
        _database = database;
        _tokenExtractor = tokenExtractor;
    }

    public async Task<PaginatedResult<MovieViewModel>> SearchAsync(FuzzySearchRequest request,
        CancellationToken cancellationToken)
    {
        var tokens = _tokenExtractor.Extract(request.Query, FieldType.Title, useNgrams: true);

        if (tokens.Count == 0)
        {
            var movies = await _database.Store<Movie>().ListAsync(cancellationToken: cancellationToken);
            return PaginatedResult<MovieViewModel>.Create(movies.Select(MovieViewModel.Create), 1);
        }
        
        var finalResults = new List<MovieViewModel>();

        var ngramArray = tokens.Where(token => token.TermType.IsNgram()).Select(token => token.Term).ToArray();

        /*var jaccardResult = await indexQueryable
            .Select(movie => new
            {
                Movie = movie,
                Score = SqlFunctions.CalculateDice(movie.Id, ngramArray)
            })
            .Where(r => r.Score >= 0.8m)
            .OrderByDescending(r => r.Score)
            .Select(r => r.Movie)
            .ToListAsync(cancellationToken);*/
        
        var matchingTermIds = _database.Store<Term>()
            .AsQueryable()
            .Where(t => ngramArray.Contains(t.TermText) && (t.TermType == TermType.CharactersNgram || t.TermType == TermType.WordNgram))
            .Select(t => t.Id);
        
        var preFilteredMovieIds = _database.Store<TermIndex>()
            .AsQueryable()
            .Where(ti => matchingTermIds.Contains(ti.TermId))
            .Select(ti => ti.MovieId)
            .Distinct();

        var allCandidates = await _database.Store<Movie>()
            .AsQueryable()
            .Where(movie => preFilteredMovieIds.Contains(movie.Id))
            .Select(movie => new MovieScoreResult
            {
                Movie = movie,
                Score = SqlFunctions.CalculateDice(movie.Id, ngramArray, 5)
            })
            .ToListAsync(cancellationToken);

        var filteredCandidates = allCandidates.OrderByDescending(r => r.Score).Where(r => r.Score > 0.3m).ToList();

        var exactMatchCandidates = filteredCandidates.Where(r => r.Score >= 0.7m).ToList();
       
        if (exactMatchCandidates.Count == 0)
        {
            var fuzzyToAdd = filteredCandidates.Where(r => r.Score is >= 0.5m and < 0.7m).ToList();
            finalResults.AddRange(fuzzyToAdd.Select(MovieViewModel.Create));
            
            if (fuzzyToAdd.Count == 0)
            {
                var broadToAdd = filteredCandidates.Where(r => r.Score is >= 0.3m and < 0.5m).ToList();
                finalResults.AddRange(broadToAdd.Select(MovieViewModel.Create));
            }
        }
        else
        {
            finalResults.AddRange(exactMatchCandidates.Select(MovieViewModel.Create));
        }

        var sortedResults = finalResults.OrderByDescending(r => r.Score);
        return PaginatedResult<MovieViewModel>.Create(sortedResults, request.PageNumber);
    }
}