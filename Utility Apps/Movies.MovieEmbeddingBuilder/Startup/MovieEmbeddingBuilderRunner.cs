using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.MovieEmbeddingBuilder.Startup;

public class MovieEmbeddingBuilderRunner(IDatabase<MovieDbContext> database, IVectorGenerator vectorGenerator)
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            const int maxConcurrentVectorizations = 5;
            const int dbInsertBatchSize = 1000;
            
            var embeddingMovieIds = await database.Store<MovieEmbedding>()
                .AsQueryable()
                .Select(e => e.MovieId)
                .ToListAsync(cancellationToken: cancellationToken);
            
            var movies = await database.Store<Movie>()
                .AsQueryable()
                .Where(m => !embeddingMovieIds.Contains(m.Id))
                .ToListAsync(cancellationToken: cancellationToken);
            
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = maxConcurrentVectorizations
            };
            
            var batchTracker = new ConcurrentDictionary<int, MovieEmbedding>();
            var moviesProcessed = 0;
            
            await Parallel.ForEachAsync(movies, parallelOptions, async (movie, ct) =>
            {
                var vector = await vectorGenerator.VectorizeAsync(movie.ToString(), ct);

                var embedding = new MovieEmbedding
                {
                    MovieId = movie.Id,
                    Vector = vector
                };
                
                batchTracker.TryAdd(movie.Id, embedding);
                Console.WriteLine($"Embedding generated for {movie.Id} | {movie.Title}");
                
                var currentProcessedCount = Interlocked.Increment(ref moviesProcessed);
                
                if (currentProcessedCount % dbInsertBatchSize == 0)
                {
                    await CommitBatch(batchTracker, dbInsertBatchSize, cancellationToken: ct);
                }
            });
            
            if (batchTracker.Count > 0)
            {
                await CommitBatch(batchTracker, batchTracker.Count, isFinal: true, cancellationToken: cancellationToken);
            }

            Console.WriteLine("All embeddings have been generated and stored.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(-1);
        }
    }
    
    private async Task CommitBatch(
        ConcurrentDictionary<int, MovieEmbedding> batchTracker,
        int targetCount,
        bool isFinal = false,
        CancellationToken cancellationToken = default)
    {
        var commitSemaphore = new SemaphoreSlim(1, 1);
        await commitSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (batchTracker.Count == 0 || (!isFinal && batchTracker.Count < targetCount))
            {
                return;
            }
            
            var itemsToCommit = batchTracker.Values
                .Take(targetCount)
                .ToList();
            
            var committedIds = itemsToCommit.Select(e => e.MovieId).ToList();
            
            await database.Store<MovieEmbedding>().BulkAddAsync(itemsToCommit, cancellationToken: cancellationToken);
        
            Console.WriteLine($"--- Committed {(isFinal ? "FINAL" : "BATCH")} of {itemsToCommit.Count} embeddings. ---");
            
            foreach (var id in committedIds)
            {
                batchTracker.TryRemove(id, out _);
            }
        }
        finally
        {
            commitSemaphore.Release();
        }
    }
}