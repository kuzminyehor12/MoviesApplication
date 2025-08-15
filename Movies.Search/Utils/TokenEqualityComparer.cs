using Movies.Search.Models;

namespace Movies.Search.Utils;

public class TokenEqualityComparer : IEqualityComparer<Token>
{
    public bool Equals(Token? x, Token? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Term == y.Term && x.Type == y.Type;
    }

    public int GetHashCode(Token obj)
    {
        return HashCode.Combine(obj.Term, (int)obj.Type);
    }
    
    public static IEqualityComparer<Token> DefaultComparer { get; } = new TokenEqualityComparer();
}