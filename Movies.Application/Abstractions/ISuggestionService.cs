using Movies.Application.Models;

namespace Movies.Application.Abstractions;

public interface ISuggestionService
{
    Task<IEnumerable<Suggestion>> GetSuggestionsAsync(string query, CancellationToken cancellationToken);
}