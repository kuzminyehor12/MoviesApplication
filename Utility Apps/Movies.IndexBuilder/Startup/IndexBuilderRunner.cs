using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Core.Entities;
using Movies.IndexBuilder.Configurations;
using Movies.Persistence.Infrastructure;

namespace Movies.IndexBuilder.Startup;

public class IndexBuilderRunner(
    IDatabase<MovieDbContext> database,
    IServiceScopeFactory scopeFactory,
    CsvConfiguration csvConfiguration,
    IOptions<IndexingDataConfiguration> indexingDataConfiguration)
{
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (indexingDataConfiguration.Value is null)
        {
            throw new Exception("Configuration cannot be found.");
        }
        Console.WriteLine("Cleaning up database...");
        
        await database.DropDatabaseAsync(cancellationToken);
        
        Console.WriteLine("Initializing database...");
        
        await database.CreateDatabaseAsync(cancellationToken);
        
        Console.WriteLine("Indexing has been started!");
        
        var moviesData = Directory.EnumerateFiles(
            indexingDataConfiguration.Value.DirectoryPath, 
            indexingDataConfiguration.Value.SearchPattern ?? string.Empty);

        var store = database.Store<Movie>().AsQueryable();

        var ids = store
            .Where(m => m.VoteAverage > 5)
            .Where(m => m.VoteCount > 100)
            .Select(m => m.Id)
            .ToList();
        
        var movies = await database.Store<Movie>().FilterAsync(m => m.VoteAverage > 5, cancellationToken);
        
        var indexingTasks = new List<Task>();

        int totalCount = 0;
        
        foreach (var file in moviesData)
        {
            var chunkIndexingTask = Task.Run(async () =>
            {
                using var streamReader = File.OpenText(file);
                using var csvReader = new CsvReader(streamReader, csvConfiguration);
                csvReader.Context.RegisterClassMap<CsvMovieMap>();
                
                await foreach (var movie in csvReader.GetRecordsAsync<Movie>(cancellationToken))
                {
                    using var scope = scopeFactory.CreateScope();
                    var scopedDbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
                    Interlocked.Increment(ref totalCount);
                    Console.WriteLine($"Processing {movie.Title} | {movie.ReleaseDate?.ToShortDateString() ?? "[NO DATE]"}");
                }
            }, cancellationToken);
            
            indexingTasks.Add(chunkIndexingTask);
        }
        
        await Task.WhenAll(indexingTasks);
        
        Console.WriteLine($"Indexing has been completed! {totalCount} documents was indexed.");
    }
}