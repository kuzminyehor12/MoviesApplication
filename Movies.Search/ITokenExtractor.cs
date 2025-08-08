using Movies.Search.Models;

namespace Movies.Search;

public interface ITokenExtractor
{
    IReadOnlySet<Token> Extract(string text, bool useNgrams = false, bool includeWholeString = false);
    
    IReadOnlySet<Token> ExtractFromCollection(IEnumerable<string> terms, bool includeWholeStringPerItem = false);
}