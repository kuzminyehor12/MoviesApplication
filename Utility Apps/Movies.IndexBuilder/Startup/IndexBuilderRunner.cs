using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Core.Entities;
using Movies.IndexBuilder.Configurations;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.IndexBuilder.Startup;

// TODO: rewrite Console logs using ILoggerFactory
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

            foreach (var file in moviesData)
            {
                Task chunkIndexingTask = IndexFile(file, cancellationToken);
                indexingTasks.Add(chunkIndexingTask);
            }

            await Task.WhenAll(indexingTasks);

            Console.WriteLine($"Indexing has been completed! {_totalCount} documents was indexed.");
        }
        catch (OperationCanceledException)
        {
            await database.DropDatabaseAsync(CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await database.DropDatabaseAsync(CancellationToken.None);
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
            
             var parallelOptions = new ParallelOptions
             {
                 MaxDegreeOfParallelism = Environment.ProcessorCount, 
                 CancellationToken = cancellationToken
             };
             
             await Parallel.ForEachAsync(csvReader.GetRecordsAsync<Movie>(cancellationToken), parallelOptions, async (movie, ct) =>
             {
                 // Step 0: extract terms
                 // Fields: title(n-grams + exact match), overview, keywords, genres, cast members, crew member
                 // Step 1: do stemming
                 // Step 2: remove stop words
                 // Step 3: create n-grams
                 // Step 4: add terms to TermIndex table
                 using var scope = scopeFactory.CreateScope();
                 var scopedDatabase = scope.ServiceProvider.GetRequiredService<IDatabase<MovieDbContext>>();
                 
                 await scopedDatabase.Store<Movie>().AddAsync(movie, ct);
                 
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
                         t => t.TermText == token.Term && t.TermType == token.Type, 
                         () => new()
                         {
                             TermText = token.Term, 
                             TermType = token.Type
                         }, 
                         ct);
                     
                     var termIndex = new TermIndex
                     {
                         TermId = indexingTerm.Id,
                         MovieId = movie.Id,
                         TermFrequency = token.Frequency,
                         TermPositions = token.Positions
                     };
                     
                     await scopedDatabase.Store<TermIndex>().AddAsync(termIndex, ct);
                 }
                 
                 Interlocked.Increment(ref _totalCount);
             });
             // Step 5: 
             // add tf-idf score to TermVector table
         });
    }
}