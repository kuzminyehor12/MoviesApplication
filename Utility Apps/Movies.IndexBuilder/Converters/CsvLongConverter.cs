using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Movies.IndexBuilder.Converters;

public class CsvLongConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return default(long);
        }

        if (double.TryParse(text, out var doubleValue))
        {
            return Convert.ToInt64(doubleValue);
        }
        
        if (decimal.TryParse(text, out var decimalValue))
        {
            return Convert.ToInt64(decimalValue);
        }

        return default(long);
    }
}