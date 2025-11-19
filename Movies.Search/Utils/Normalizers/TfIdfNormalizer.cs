namespace Movies.Search.Utils.Normalizers;

internal class TfIdfNormalizer : DefaultNormalizer
{
    protected override string[] GetNoise() =>  ["*", "(", ")", "{", "}", "\"", "'"];
}