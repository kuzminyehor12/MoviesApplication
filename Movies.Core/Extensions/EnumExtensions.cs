using Movies.Core.Enums;

namespace Movies.Core.Extensions;

public static class EnumExtensions
{
    public static bool NotNgram(this TermType termType) =>
        !IsNgram(termType);
    
    public static bool IsNgram(this TermType termType) =>
        termType == TermType.CharactersNgram || termType == TermType.WordNgram;
}