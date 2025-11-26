using Movies.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Movies.Persistence.Batching;

public  class BatchProcessor
{
    public static async IAsyncEnumerable<List<T>> GetBatchAsync<T>(IAsyncEnumerable<T> source, int batchSize)
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
        const int connectionPoolSize = 25;
        var semaphore = new SemaphoreSlim(connectionPoolSize);
        
        while (true)
        {
            List<TEntity> batch = new List<TEntity>();
            bool success = false;
            int retryCount = 0;
            
            while (retryCount <= 3 && !success)
            {
                try
                {
                    batch = await source
                        .AsNoTracking()
                        .Skip(processing)
                        .Take(batchSize)
                        .ToListAsync();
                    
                    success = true;
                }
                catch
                {
                    Console.WriteLine($"info: Retrying #{retryCount}...");
                    retryCount++;
                }
            }
            
            if (!batch.Any())
                break;
            
            await semaphore.WaitAsync();
            
            tasks.Add(Task.Run(async () => {
                try
                {
                    await action(batch);
                } 
                finally
                {
                    semaphore.Release();
                }
            }));
            
            Interlocked.Add(ref processing, batch.Count);
        }
        
        await Task.WhenAll(tasks);
    }
}