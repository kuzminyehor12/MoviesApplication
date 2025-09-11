using Movies.Core.Enums;
using Movies.Search.Utils.Normalizers;

namespace Movies.Search.Utils.Generators;

public class NgramTokenGenerator(FieldType fieldType) : ITokenGenerator
{
    public TokenCollection Generate(string text)
    {
        ITextNormalizer normalizer = new NgramNormalizer();
        string normalizedText = normalizer.Normalize(text);
        
        var charNgrams = NgramGenerator.GenerateByCharacters(normalizedText);
        var ngramCharacterTokens = TokenCollection.Create(TermType.CharactersNgram, fieldType, charNgrams);
        
        var wordNgrams = NgramGenerator.GenerateByWords(normalizedText);
        var ngramWordTokens = TokenCollection.Create(TermType.WordNgram, fieldType, wordNgrams);
        
        return new TokenCollection(ngramCharacterTokens.Concat(ngramWordTokens));
    }
}