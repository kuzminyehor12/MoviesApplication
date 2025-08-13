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

    private int _totalCount;
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (indexingDataConfiguration.Value is null)
        {
            throw new Exception("Configuration cannot be found.");
        }

        try
        {
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
                Task chunkIndexingTask = IndexFile(file, cancellationToken);
                indexingTasks.Add(chunkIndexingTask);
            }

            await Task.WhenAll(indexingTasks);

            Console.WriteLine($"Indexing has been completed! {totalCount} documents was indexed.");
        }
        catch (OperationCanceledException)
        {
            await database.DropDatabaseAsync(CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(-1);
        }
    }

    private Task IndexFile(string file, CancellationToken cancellationToken = default)
    {
         return Task.Run(async () =>
         {
             cancellationToken.ThrowIfCancellationRequested();
             
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
                 
                 var indexingMovie = await scopedDatabase.Store<Movie>().GetOrAddAsync(m => m.Id == movie.Id, () => movie, cancellationToken);
                 
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
                     Term indexingTerm = await scopedDatabase.Store<Term>().GetOrAddAsync(
                         t => t.TermText == token.Term, 
                         () => new() { TermText = token.Term, TermType = token.Type }, 
                         cancellationToken);
                     
                     var termIndex = new TermIndex
                     {
                         TermId = indexingTerm.Id,
                         MovieId = indexingMovie.Id,
                         TermFrequency = token.Frequency,
                         TermPositions = token.Positions
                     };
                     
                     await scopedDatabase.Store<TermIndex>().TryAddAsync(termIndex, cancellationToken);
                 }
                 
                 Interlocked.Increment(ref _totalCount);
                 Console.WriteLine($"info: Processing {movie.Title} | {movie.ReleaseDate?.ToShortDateString() ?? "[NO DATE]"}");
             }
             // Step 5: 
             // add tf-idf score to TermVector table
         }, cancellationToken);
    }
}