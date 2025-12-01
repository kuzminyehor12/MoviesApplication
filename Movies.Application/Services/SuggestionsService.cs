using Microsoft.EntityFrameworkCore;
using Movies.Application.Abstractions;
using Movies.Application.Models;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Persistence.Infrastructure;
using Movies.Search.Utils.Normalizers;

namespace Movies.Application.Services;

public class SuggestionsService : ISuggestionService
{
    private readonly IDatabase<MovieDbContext> _database;
    private readonly ITextNormalizer _textNormalizer;

    public SuggestionsService(IDatabase<MovieDbContext> database, ITextNormalizer textNormalizer)
    {
        _database = database;
        _textNormalizer = textNormalizer;
    }

    public async Task<IEnumerable<Suggestion>> GetSuggestionsAsync(string query, CancellationToken cancellationToken)
    {
        const int suggestionsCount = 5;
        var normalizedQuery = _textNormalizer.Normalize(query);
        
        var suggestedIndex = await _database.Store<TermIndex>()
            .AsQueryable()
            .Where(index => index.TermPositions.Contains(0))
            .Include(index => index.Term)
            .Include(index => index.Movie)
            .Where(index => index.Term.TermText == normalizedQuery && (index.Term.TermType == TermType.CharactersNgram || index.Term.TermType == TermType.WordNgram))
            .Take(suggestionsCount)
            .Select(index => new Suggestion
            {
                MovieId = index.Movie.Id,
                Title = index.Movie.Title,
                VoteAverage = index.Movie.VoteAverage,
            })
            .ToListAsync(cancellationToken);

        return suggestedIndex;
    }
}