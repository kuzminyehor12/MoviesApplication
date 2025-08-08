using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Core.Entities;
using Movies.IndexBuilder.Configurations;
using Movies.Persistence.Infrastructure;
using Movies.Search;

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
        
        var indexingTasks = new List<Task>();

        int totalCount = 0;
        
        foreach (var file in moviesData)
        {
            var chunkIndexingTask = Task.Run(async () =>
            {
                using var streamReader = File.OpenText(file);
                using var csvReader = new CsvReader(streamReader, csvConfiguration);
                csvReader.Context.RegisterClassMap<CsvMovieMap>();
                
                using var scope = scopeFactory.CreateScope();
                var scopedDatabase = scope.ServiceProvider.GetRequiredService<IDatabase<MovieDbContext>>();
                
                await foreach (var movie in csvReader.GetRecordsAsync<Movie>(cancellationToken))
                {
                    // Step 0: extract terms
                    // Fields: title(n-grams + exact match), overview, keywords, genres, cast members, crew member
                    // Step 1: do stemming
                    // Step 2: remove stop words
                    // Step 3: create n-grams
                    // Step 4: add terms to TermIndex table
                    await scopedDatabase.Store<Movie>().AddAsync(movie, cancellationToken);
                    
                    using var transientScope = scopeFactory.CreateScope();
                    var termExtractor = transientScope.ServiceProvider.GetRequiredService<ITokenExtractor>();

                    var tokens = termExtractor.Extract(movie.Title, useNgrams: true, includeWholeString: true);
                            // .Union(termExtractor.Extract(movie.Overview ?? string.Empty))
                            // .Union(termExtractor.ExtractFromCollection(movie.Keywords.Select(k => k.Name), includeWholeStringPerItem: true))
                            // .Union(termExtractor.ExtractFromCollection(movie.Genres.Select(g => g.Name)))
                            // .Union(termExtractor.ExtractFromCollection(movie.CastMembers.Select(cast => cast.Name), includeWholeStringPerItem: true))
                            // .Union(termExtractor.ExtractFromCollection(movie.Keywords.Select(crew => crew.Name), includeWholeStringPerItem: true));

                    foreach (var token in tokens)
                    {
                        var term = new Term
                        {
                            TermText = token.Term,
                            TermType = token.Type,
                        };
                        
                        await scopedDatabase.Store<Term>().AddAsync(term, cancellationToken);

                        var termIndex = new TermIndex
                        {
                            Id = (term.Id, movie.Id),
                            TermFrequency = token.Frequency,
                            TermPositions = token.Positions
                        };

                        await scopedDatabase.Store<TermIndex>().AddAsync(termIndex, cancellationToken);
                    }
                    
                    Interlocked.Increment(ref totalCount);
                    Console.WriteLine($"Processing {movie.Title} | {movie.ReleaseDate?.ToShortDateString() ?? "[NO DATE]"}");
                }

                // Step 5: 
                // add tf-idf score to TermVector table
                await scopedDatabase.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            
            indexingTasks.Add(chunkIndexingTask);
        }
        
        await Task.WhenAll(indexingTasks);
        
        Console.WriteLine($"Indexing has been completed! {totalCount} documents was indexed.");
    }
}