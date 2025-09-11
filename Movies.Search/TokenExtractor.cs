using Movies.Core.Enums;
using Movies.Search.Models;
using Movies.Search.Utils.Generators;
using Movies.Search.Utils.Normalizers;

namespace Movies.Search;

public class TokenExtractor : ITokenExtractor
{
    public IReadOnlySet<Token> Extract(string text, FieldType fieldType, bool useNgrams = false, bool includeWholeString = false)
    {
        if (string.IsNullOrWhiteSpace(text))
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
                TermType = TermType.WholeString,
                FieldType = FieldType.Title,
                Frequency = 1
            };
            
            tokens.Add(wholeStringToken);
        }

        ITokenGenerator tokenGenerator = useNgrams ? new NgramTokenGenerator(fieldType) : new TfIdfTokenGenerator(fieldType);

        tokens.AddRange(tokenGenerator.Generate(normalizedText));

        return tokens.ToHashSet();
    }
    
    public IReadOnlySet<Token> Extract(IEnumerable<string?>? terms, FieldType fieldType, bool includeWholePerString = false)
    {
        if (terms is null || !terms.Any())
        {
            return new HashSet<Token>();
        }
        
        var tokens = new TokenCollection();
        ITextNormalizer textNormalizer = new DefaultNormalizer();

        if (includeWholePerString)
        {
            foreach (var term in terms)
            {
                string normalizedTerm = textNormalizer.Normalize(term);
                
                var wholeStringToken = new Token
                {
                    Term = normalizedTerm,
                    TermType = TermType.WholeString,
                    FieldType = fieldType,
                    Frequency = 1
                };
            
                tokens.Add(wholeStringToken);
            }
            
            return tokens.ToHashSet();
        }
        
        ITokenGenerator tokenGenerator = new TfIdfTokenGenerator(fieldType);

        foreach (var term in terms)
        {
            if (string.IsNullOrEmpty(term))
            {
                continue;
            }
            
            tokens.AddRange(tokenGenerator.Generate(term));
        }
        
        return tokens.ToHashSet();
    }
}