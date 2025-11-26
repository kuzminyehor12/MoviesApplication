namespace Movies.Search;

public interface IVectorGenerator
{
    Task<float[]> VectorizeAsync(string input, CancellationToken cancellationToken = default);
}