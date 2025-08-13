namespace Movies.Search.Utils.Generators;

public static class NgramGenerator
{
    private const int CharacterNgramLength = 3;
    
    private const int WordNgramLength = 2;
    
    public static string[] GenerateByCharacters(string? term)
    {
        var ngrams = new List<string>();
        
        if (string.IsNullOrWhiteSpace(term) || term.Length < CharacterNgramLength)
        {
            return [];
        }

        for (int i = 0; i <= term.Length - CharacterNgramLength; i++)
        {
            ngrams.Add(term.Substring(i, CharacterNgramLength));
        }

        return ngrams.ToArray();
    }
    
    public static string[] GenerateByWords(string? term)
    {
        var ngrams = new List<string>();
        
        if (string.IsNullOrWhiteSpace(term))
        {
            return [];
        }

        var words = term.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        
        if (words.Length < WordNgramLength)
        {
            return [];
        }

        for (int i = 0; i <= words.Length - WordNgramLength; i++)
        {
            ngrams.Add(string.Join(" ", words.Skip(i).Take(WordNgramLength)));
        }

        return ngrams.ToArray();
    }
}