using Microsoft.Extensions.AI;

namespace Movies.Search;

public class VectorGenerator(IEmbeddingGenerator<string, Embedding<float>> embeddingClient) : IVectorGenerator
{
    public async Task<float[]> VectorizeAsync(string input, CancellationToken cancellationToken = default)
    {
        var vector = await embeddingClient.GenerateVectorAsync(input, cancellationToken: cancellationToken);
        return vector.ToArray();
    }
}