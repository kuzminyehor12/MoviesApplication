using Movies.Core.Enums;
using Movies.Search.Models;
using Movies.Search.Utils.Generators;
using Movies.Search.Utils.Normalizers;

namespace Movies.Search;

// TODO: write unit tests for this class
public class TokenExtractor : ITokenExtractor
{
    public IReadOnlySet<Token> Extract(string text, bool useNgrams = false, bool includeWholeString = false)
    {
        // TODO: 1. Lowercase
        // 2. Trim
        // 3. Remove\Replace punctuations
        // 4. Normalize apostrophes
        // 5. Handle Diacritics/Accents
        
        if (string.IsNullOrEmpty(text))
        {
            return new HashSet<Token>();
        }
        
        var tokens = new TokenCollection();
        ITextNormalizer textNormalizer = new DefaultNormalizer();
        
        string normalizedText = textNormalizer.Normalize(text);

        if (includeWholeString)
        {
            var wholeStringToken = new Token
            {
                Term = normalizedText,
                Type = TermType.WholeString,
                Frequency = 1
            };
            
            tokens.Add(wholeStringToken);
        }

        ITokenGenerator tokenGenerator = useNgrams ? new NgramTokenGenerator() : new TfIdfTokenGenerator();

        tokens.AddRange(tokenGenerator.Generate(normalizedText));

        return tokens.ToHashSet();
    }
    
    public IReadOnlySet<Token> ExtractFromCollection(IEnumerable<string> terms, bool includeWholeStringPerItem = false)
    {
        throw new NotImplementedException();
    }
}