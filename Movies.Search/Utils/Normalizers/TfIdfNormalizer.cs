namespace Movies.Search.Utils.Normalizers;

internal class TfIdfNormalizer : DefaultNormalizer
{
    public override string Normalize(string text)
    {
        string normalized = base.Normalize(text);
        return normalized;
    }
}