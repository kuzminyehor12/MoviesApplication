using Movies.Core.Enums;

namespace Movies.Search;

public interface ITermExtractor
{
    (string Term, TermType Type)[] Extract(string term, bool withNgrams = false, bool includeWholeString = false);
}