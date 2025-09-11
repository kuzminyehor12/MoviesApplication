using Movies.Core.Enums;
using Movies.Search.Utils.Normalizers;
using Porter2StemmerStandard;

namespace Movies.Search.Utils.Generators;

public class TfIdfTokenGenerator(FieldType fieldType) : ITokenGenerator
{
    private const string StopWordsFilePath = @"Data\EN-Stopwords.txt";

    private static string[] StopWords => 
        File.ReadAllLines(StopWordsFilePath)
        .Where(word => !string.IsNullOrWhiteSpace(word))
        .ToArray();
    
    public TokenCollection Generate(string text)
    {
        ITextNormalizer normalizer = new TfIdfNormalizer();
        string normalizedText = normalizer.Normalize(text);

        if (string.IsNullOrWhiteSpace(normalizedText))
        {
            return new TokenCollection();
        }
        
        string[] terms = normalizedText.Split(' ');

        var termsWithNoStopWords = terms.Where(term => !StopWords.Contains(term));
        
        EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
        string[] stemmedTerms = termsWithNoStopWords.Select(Stem).ToArray();
        
        return TokenCollection.Create(TermType.WholeWord, fieldType, stemmedTerms);

        string Stem(string term)
        {
            try
            {
                StemmedWord stemmedTerm = stemmer.Stem(term);
                return stemmedTerm.Value;
            }
            catch (Exception) // term does not belong to English
            {
                return term;
            }
        }
    }
}