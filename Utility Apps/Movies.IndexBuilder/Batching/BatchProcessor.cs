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

    public static async Task BatchAsync<T>(IEnumerable<T> source, int batchSize, Func<IList<T>, Task> action)
    {
        int processed = 0;
        var tasks = new List<Task>();
        
        while (true)
        {
            var batch = source
                .Skip(processed)
                .Take(batchSize)
                .ToList();

            if (!batch.Any())
                break;
            
            tasks.Add(action(batch));
            
            Interlocked.Add(ref processed, batch.Count);
        }
        
        await Task.WhenAll(tasks);
    }
}