using System.Reflection;
using System.Text.RegularExpressions;
using Movies.Core.Enums;
using Movies.Search.Models;
using Movies.Search.Utils.Normalizers;
using Porter2StemmerStandard;

namespace Movies.Search.Utils.Generators;

public class TfIdfTokenGenerator(FieldType fieldType) : ITokenGenerator
{
    private const string StopWordsFilePath = @"Data\EN-Stopwords.txt";

    private static string[] StopWords => 
        File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), StopWordsFilePath))
        .Where(word => !string.IsNullOrWhiteSpace(word))
        .ToArray();
    
    public TokenCollection Generate(string text)
    {
        const string termPattern = @"(\[[^\]]+\])|\w+";
        EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
        var terms = new List<string>();
        var wholeStrings = new List<string>();

        if (string.IsNullOrWhiteSpace(text))
        {
            return new TokenCollection();
        }
        
        var matches = Regex.Matches(text, termPattern);
        
        foreach (Match match in matches)
        {
            ITextNormalizer? normalizer;
            if (match.Groups[1].Success)
            {
                string phrase = match.Groups[1].Value.Trim('[', ']');
                string[] termsWithinPhrase = phrase.Split(' ');
                normalizer = new DefaultNormalizer();

                foreach (string term in termsWithinPhrase)
                {
                    wholeStrings.Add(normalizer.Normalize(term));
                }
            }
            else
            {
                normalizer = new TfIdfNormalizer();
                string normalizedText = normalizer.Normalize(match.Value);
                terms.Add(Stem(normalizedText));
            }
        }

        var termsWithNoStopWords = terms.Where(term => !StopWords.Contains(term));
        
        return TokenCollection.Create(TermType.WholeWord, fieldType, termsWithNoStopWords.Union(wholeStrings).ToArray());

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