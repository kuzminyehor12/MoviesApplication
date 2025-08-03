using Movies.Core.Enums;

namespace Movies.Search;

public class TermExtractor : ITermExtractor
{
    public (string Term, TermType Type)[] Extract(string term, bool withNgrams = false, bool includeWholeString = false)
    {
        throw new NotImplementedException();
    }
}