using Movies.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Movies.Persistence.Batching;

public  class BatchProcessor
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

    public static async Task BatchAsync<TEntity>(IQueryable<TEntity> source, int batchSize, Func<IList<TEntity>, Task> action)
        where TEntity : class, IEntity
    {
        int processing = 0;
        var tasks = new List<Task>();
        
        while (true)
        {
            var batch = await source
                .AsNoTracking()
                .Skip(processing)
                .Take(batchSize)
                .ToListAsync();

            if (!batch.Any())
                break;
            
            tasks.Add(action(batch));
            Interlocked.Add(ref processing, batch.Count);
        }
        
        await Task.WhenAll(tasks);
    }
}