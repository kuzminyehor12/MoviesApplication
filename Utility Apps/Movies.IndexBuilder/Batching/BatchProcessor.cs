namespace Movies.IndexBuilder.Batching;

public static class BatchProcessor
{
    public static async IAsyncEnumerable<List<T>> BatchAsync<T>(IAsyncEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        
        await foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }
        
        if (batch.Count > 0)
        {
            yield return batch;
        }
    }
}