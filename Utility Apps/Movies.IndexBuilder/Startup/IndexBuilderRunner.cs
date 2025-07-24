using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using Movies.Core.Entities;
using Movies.Persistence.Infrastructure;

namespace Movies.IndexBuilder.Startup;

public class IndexBuilderRunner(
    MovieDbContext dbContext, 
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
        
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        
        Console.WriteLine("Initializing database...");
        
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        
        Console.WriteLine("Indexing has been started!");
        
        var moviesData = Directory.EnumerateFiles(
            indexingDataConfiguration.Value.DirectoryPath, 
            indexingDataConfiguration.Value.SearchPattern ?? string.Empty);
        
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