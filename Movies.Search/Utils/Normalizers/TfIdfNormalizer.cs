namespace Movies.Search.Utils.Normalizers;

internal class TfIdfNormalizer : DefaultNormalizer
{
    // TODO: remove stop words
    // TODO: do lemmatizing
    public override string Normalize(string text)
    {
        string normalized = base.Normalize(text);
        return normalized;
    }
}