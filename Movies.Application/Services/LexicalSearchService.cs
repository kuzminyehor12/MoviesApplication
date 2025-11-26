using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Application.Requests;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.Application.Services;

public class LexicalSearchService : ILexicalSearchService
{
    private readonly IDatabase<MovieDbContext> _database;
    private readonly ITokenExtractor _tokenExtractor;

    public LexicalSearchService(IDatabase<MovieDbContext> database, ITokenExtractor tokenExtractor)
    {
        _database = database;
        _tokenExtractor = tokenExtractor;
    }

    public async Task<PaginatedResult<MovieViewModel>> SearchAsync(LexicalSearchRequest request, CancellationToken cancellationToken)
    {
        var tokens = _tokenExtractor.Extract(request.Query);

        var termArray = tokens.Select(t => t.Term).ToArray();
        
        var matchingTermIds = _database.Store<Term>()
            .AsQueryable()
            .Where(t => termArray.Contains(t.TermText) && (t.TermType != TermType.CharactersNgram || t.TermType != TermType.WordNgram))
            .Select(t => t.Id);
        
        const decimal highWeight = 2.5m;
        const decimal maxWeight = 3m;
        const decimal defaultWeight = 1.0m;
        
        const decimal missingFieldPenaltyMultiplier = 0.5m;
        
        var aggregatedScores = _database.Store<TermVector>()
            .AsQueryable()
            .Join(
                _database.Store<TermFieldMap>().AsQueryable(),
                vector => new { vector.MovieId, vector.TermId },
                fieldMap => new { fieldMap.MovieId, fieldMap.TermId },
                (vector, fieldMap) => new
                {
                    vector.TermId,
                    vector.MovieId,
                    vector.Score,
                    fieldMap.FieldType
                }
            )
            .Where(joined => matchingTermIds.Contains(joined.TermId))
            .GroupBy(joined => joined.MovieId)
            .Select(g => new
            {
                MovieId = g.Key,
                BaseScore = g.Sum(joined =>
                    (decimal)joined.Score * (joined.FieldType == FieldType.Title 
                        ? maxWeight
                        : joined.FieldType == FieldType.Actor || joined.FieldType == FieldType.Character || joined.FieldType == FieldType.Genre || joined.FieldType == FieldType.Crew 
                            ? highWeight
                            : defaultWeight)
                )
            })
            .Join(
                _database.Store<Movie>().AsQueryable(),
                agg => agg.MovieId,
                movie => movie.Id,
                (agg, movie) => new
                {
                    agg.MovieId,
                    agg.BaseScore,
                    movie.Keywords,
                    movie.CastMembers,
                    movie.Genres,
                    movie.CrewMembers,
                    movie.TagLine,
                    movie.Overview 
                }
            )
            .Select(final => new
            {
                final.MovieId,
                TotalScore = final.BaseScore *
                             (
                                 string.IsNullOrEmpty(final.TagLine) 
                                     ? missingFieldPenaltyMultiplier 
                                     : 1.0m
                             ) * (
                                 string.IsNullOrEmpty(final.Overview)
                                     ? missingFieldPenaltyMultiplier
                                     : 1.0m
                             ) * (
                                 final.Keywords == null || final.Keywords.Length == 0
                                     ? missingFieldPenaltyMultiplier 
                                     : 1.0m
                             ) * (
                                 final.Genres == null || final.Genres.Length == 0
                                     ? missingFieldPenaltyMultiplier 
                                     : 1.0m 
                             ) * (
                                 final.CrewMembers == null || final.CrewMembers.Length == 0
                                     ? missingFieldPenaltyMultiplier 
                                     : 1.0m 
                             ) * (
                                 final.CastMembers == null || final.CastMembers.Length == 0
                                     ? missingFieldPenaltyMultiplier 
                                     : 1.0m 
                             )
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
        return PaginatedResult<MovieViewModel>.Create(viewModels, request.PageNumber);
    }
}