using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Movies.Search.Utils.Normalizers;

public class DefaultNormalizer : ITextNormalizer
{
    private static readonly Regex PunctuationPattern = new (@"[^\p{L}\p{N}\s]", RegexOptions.Compiled);
    
    private static readonly Regex WhiteSpacePattern = new (@"\s+", RegexOptions.Compiled);
    
    public virtual string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }
        
        text = text.ToLowerInvariant().Trim();

        // Handle Diacritics/Accents (e.g., "été" -> "ete")
        text = text.Normalize(NormalizationForm.FormD);
        
        var sb = new StringBuilder();
        
        foreach (char c in text)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);

            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        
        text = sb.ToString().Normalize(NormalizationForm.FormC);
        
        text = text.Replace("'", string.Empty).Replace("’", string.Empty);
        text = PunctuationPattern.Replace(text, " ");
        text = WhiteSpacePattern.Replace(text, " ");
        
        return text.Trim();
    }
}