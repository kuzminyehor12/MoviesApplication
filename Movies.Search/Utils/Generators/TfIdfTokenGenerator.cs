using Movies.Core.Enums;
using Movies.Search.Utils.Normalizers;

namespace Movies.Search.Utils.Generators;

internal class TfIdfTokenGenerator : ITokenGenerator
{
    public TokenCollection Generate(string text)
    {
        ITextNormalizer normalizer = new TfIdfNormalizer();
        string normalizedText = normalizer.Normalize(text);
        string[] terms = normalizedText.Split(' ');
        return TokenCollection.Create(TermType.WholeString, terms);
    }
}