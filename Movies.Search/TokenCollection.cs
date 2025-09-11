using Movies.Core.Enums;
using Movies.Search.Models;
using Movies.Search.Utils;
using Movies.Search.Utils.Extensions;

namespace Movies.Search;

public class TokenCollection : List<Token>
{
    public TokenCollection()
    {
        
    }

    internal TokenCollection(IEnumerable<Token> tokens)
    {
        Clear();
        
        foreach (var token in tokens)
        {
            Add(token);
        }
    }
    
    internal static TokenCollection Create(TermType type, FieldType fieldType, string[] dictionary)
    {
        TokenCollection tokenCollection = new TokenCollection();
        
        foreach (var term in dictionary)
        {
            var frequencyWithPositions = type == TermType.WholeWord ? dictionary.FrequencyWithPositions(term) : (1, []);

            var token = new Token
            {
                Term = term,
                TermType = type,
                FieldType = fieldType,
                Frequency = frequencyWithPositions.Frequency,
                Positions = frequencyWithPositions.Positions
            };
                
            tokenCollection.Add(token);
        }
        
        return tokenCollection;
    }
    
    internal HashSet<Token> ToHashSet()
    {
        return this.ToHashSet(TokenEqualityComparer.DefaultComparer);
    }
}