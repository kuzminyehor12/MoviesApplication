using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Movies.IndexBuilder.Converters;

public class CsvObjectArrayConverter<T> : TypeConverter<T>
{
    // Pattern explanation:
    // (?<!\\)'    - Matches single quote not preceded by backslash (negative lookbehind)
    // (.*?)       - Matches any characters (non-greedy)
    // (?<!\\)'    - Matches closing single quote not preceded by backslash
    private const string SingleQuotesPattern = @"(?<!\\)'(.*?)(?<!\\)'";
    
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    
    public override T? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text) || text == "[]")
        {
            return default;
        }
        
        string json = Regex.Replace(text, SingleQuotesPattern, "\"$1\"");
        
        return JsonSerializer.Deserialize<T>(json,  _options);
    }

    public override string? ConvertToString(T? value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is null)
        {
            return string.Empty;
        }
        
        return JsonSerializer.Serialize(value, _options);
    }
}