using Movies.Core.Entities;

namespace Movies.Application.Abstractions;

public interface ISuggestionService
{
    Task<IEnumerable<Movie>> GetSuggestionsAsync(string query, CancellationToken cancellationToken);
}