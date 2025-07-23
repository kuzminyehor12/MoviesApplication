using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Movies.IndexBuilder.Converters;

public class CsvIntConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return default(int);
        }

        if (double.TryParse(text, out var doubleValue))
        {
            return Convert.ToInt32(doubleValue);
        }
        
        if (decimal.TryParse(text, out var decimalValue))
        {
            return Convert.ToInt32(decimalValue);
        }

        return default(int);
    }
}