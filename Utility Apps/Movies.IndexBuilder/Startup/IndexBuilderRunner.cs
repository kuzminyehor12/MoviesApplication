using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.IndexBuilder.Batching;
using Movies.IndexBuilder.Configurations;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using Movies.Search.Models;
using Movies.Search.Utils;

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

             const int batchSize = 100;
             
             // Step 0: extract terms
             // Fields: title(n-grams + exact match), overview, keywords, genres, cast members, crew member
             // Step 1: do stemming
             // Step 2: remove stop words
             // Step 3: create n-grams
             // Step 4: add terms to TermIndex table
             
             await Parallel.ForEachAsync(BatchProcessor.BatchAsync(csvReader.GetRecordsAsync<Movie>(cancellationToken), batchSize), parallelOptions, ProcessBatch);
             // Step 5: 
             // add tf-idf score to TermVector table
         });
    }

    private async ValueTask ProcessBatch(IEnumerable<Movie> batch, CancellationToken ct = default)
    {
        using var taskScope = scopeFactory.CreateScope();
        var scopedDatabase = taskScope.ServiceProvider.GetRequiredService<IDatabase<MovieDbContext>>();
        var termExtractor = taskScope.ServiceProvider.GetRequiredService<ITokenExtractor>();
        
        var allTermIndices = new List<TermIndex>();
        var allMovies = new List<Movie>();
        var allTerms = new List<Term>();
        
        foreach (var movie in batch)
        {
            Console.WriteLine($"info: Processing {movie.Id} | \"{movie.Title}\"");
            
            IReadOnlySet<Token> tokens = termExtractor.Extract(movie.Title, useNgrams: true, includeWholeString: true);
                // .Union(termExtractor.Extract(movie.Overview ?? string.Empty))
                // .Union(termExtractor.ExtractFromCollection(movie.Keywords.Select(k => k.Name), includeWholeStringPerItem: true))
                // .Union(termExtractor.ExtractFromCollection(movie.Genres.Select(g => g.Name)))
                // .Union(termExtractor.ExtractFromCollection(movie.CastMembers.Select(cast => cast.Name), includeWholeStringPerItem: true))
                // .Union(termExtractor.ExtractFromCollection(movie.Keywords.Select(crew => crew.Name), includeWholeStringPerItem: true));
            
            var termsLookup = allTerms.ToDictionary(t => (t.TermText, t.TermType), t => t);
            var tokenLookup = new Dictionary<(string Term, TermType Type), Token>();
            
            var existingLocalTerms = allTerms.Where(term => tokens.Contains(new Token
            {
                Term = term.TermText,
                Type = term.TermType
            }, TokenEqualityComparer.DefaultComparer));
            
            var newLocalTerms = new List<Term>();
            
            foreach (var token in tokens)
            {
                if (!termsLookup.ContainsKey((token.Term, token.Type)))
                {
                    var newTerm = new Term
                    {
                        TermText = token.Term,
                        TermType = token.Type,
                    };
                    
                    newLocalTerms.Add(newTerm);
                }
                
                tokenLookup.Add((token.Term, token.Type), token);
            }
            
            var allLocalTerms = existingLocalTerms.Concat(newLocalTerms);
            foreach (var indexingTerm in allLocalTerms)
            {
                var lookupKey = (indexingTerm.TermText, indexingTerm.TermType);
                
                var termIndex = new TermIndex
                {
                    Term = indexingTerm, 
                    Movie = movie, 
                    TermFrequency = tokenLookup[lookupKey].Frequency, 
                    TermPositions = tokenLookup[lookupKey].Positions
                };
                
                allTermIndices.Add(termIndex);
            }
            
            allTerms.AddRange(newLocalTerms);
            allMovies.Add(movie);
            
            Interlocked.Increment(ref _totalCount);
        }
        
        await scopedDatabase.Store<Term>().BulkAddAsync(allTerms, ct);
        await scopedDatabase.Store<Movie>().BulkAddAsync(allMovies, ct);
        await scopedDatabase.Store<TermIndex>().BulkAddAsync(allTermIndices, ct);
    }
}