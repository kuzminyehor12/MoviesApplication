using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Core.Extensions;
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

    public async Task<PaginatedResult<MovieViewModel>> SearchAsync(FuzzySearchRequest request, CancellationToken cancellationToken)
    {
        var tokens = _tokenExtractor.Extract(request.Query, FieldType.Title, useNgrams: true);

        if (tokens.Count == 0)
        {
            var movies = await _database.Store<Movie>().ListAsync(cancellationToken: cancellationToken);
            return PaginatedResult<MovieViewModel>.Create(movies.Select(MovieViewModel.Create), request.PageNumber);
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

        var filteredCandidates = _database.Store<Movie>()
            .AsQueryable()
            .Where(movie => preFilteredMovieIds.Contains(movie.Id))
            .Select(movie => new MovieScoreResult
            {
                Movie = MovieViewModel.Create(movie),
                Score = SqlFunctions.CalculateDice(movie.Id, ngramArray, 5)
            })
            .Where(r => r.Score > 0.3m)
            .OrderByDescending(r => r.Score);

        var exactMatchCandidates = await filteredCandidates
            .Where(r => r.Score >= 0.7m)
            .Select(r => r.Movie)
            .ToListAsync(cancellationToken);
       
        if (exactMatchCandidates.Count == 0)
        {
            var fuzzyToAdd = await filteredCandidates
                    .Where(r => r.Score > 0.5m && r.Score < 0.7m)
                    .Select(r => r.Movie)
                    .ToListAsync(cancellationToken);
            
            finalResults.AddRange(fuzzyToAdd);
            
            if (fuzzyToAdd.Count == 0)
            {
                var broadToAdd = await filteredCandidates
                    .Where(r => r.Score >= 0.3m && r.Score <= 0.5m)
                    .Select(r => r.Movie)
                    .ToListAsync(cancellationToken);
                
                finalResults.AddRange(broadToAdd);
            }
        }
        else
        {
            finalResults.AddRange(exactMatchCandidates);
        }

        return PaginatedResult<MovieViewModel>.Create(finalResults, request.PageNumber);
    }
}