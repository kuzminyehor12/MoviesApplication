using Movies.Core.Enums;
using Movies.Search.Models;

namespace Movies.Search;

public interface ITokenExtractor
{
    IReadOnlySet<Token> Extract(string text, FieldType fieldType = FieldType.None, bool useNgrams = false, bool includeWholeString = false);
    
    IReadOnlySet<Token> Extract(IEnumerable<string?>? terms, FieldType fieldType = FieldType.None, bool includeWholePerString = false);
}