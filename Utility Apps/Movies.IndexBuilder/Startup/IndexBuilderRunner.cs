using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Core.Extensions;
using Movies.IndexBuilder.Configurations;
using Movies.Persistence.Batching;
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

            await PopulateTfIdfScoreAsync(cancellationToken);

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

             const int batchSize = 1000;
             var batches = BatchProcessor.BatchAsync(csvReader.GetRecordsAsync<Movie>(cancellationToken), batchSize);
             await Parallel.ForEachAsync(batches, parallelOptions, ProcessBatch);
         });
    }

    private async ValueTask ProcessBatch(IEnumerable<Movie> batch, CancellationToken ct = default)
    {
        using var taskScope = scopeFactory.CreateScope();
        var scopedDatabase = taskScope.ServiceProvider.GetRequiredService<IDatabase<MovieDbContext>>();
        var tokenExtractor = taskScope.ServiceProvider.GetRequiredService<ITokenExtractor>();
        
        var termIndicesBatch = new List<TermIndex>();
        var termVectorsBatch = new List<TermVector>();
        var moviesBatch = new List<Movie>();
        var termsBatch = new List<Term>();
        
        foreach (var movie in batch)
        {
            Console.WriteLine($"info: Processing {movie.Id} | \"{movie.Title}\"");
            
            IReadOnlySet<Token> tokens = tokenExtractor.Extract(movie.Title, FieldType.Title, useNgrams: true, includeWholeString: true)
                .Union(tokenExtractor.Extract(movie.Overview ?? string.Empty, FieldType.Overview))
                .Union(tokenExtractor.Extract(movie.Keywords?.Select(k => k.Name) ?? [], FieldType.Keyword, includeWholePerString: true))
                .Union(tokenExtractor.Extract(movie.Genres?.Select(g => g.Name) ?? [], FieldType.Genre, includeWholePerString: true))
                .Union(tokenExtractor.Extract(movie.CastMembers?.Select(cast => cast.Name).Take(3) ?? [], FieldType.Actor, includeWholePerString: true))
                .Union(tokenExtractor.Extract(movie.CastMembers?.Where(cast => !string.IsNullOrWhiteSpace(cast.Character)).Select(cast => cast.Character).Take(3) ?? [], FieldType.Character, includeWholePerString: true))
                .Union(tokenExtractor.Extract(movie.CrewMembers?.Select(cast => cast.Name).Take(2) ?? [], FieldType.Crew, includeWholePerString: true))
                .ToHashSet(TokenEqualityComparer.DefaultComparer);
                
            var tokenLookup = new Dictionary<(string Term, TermType Type), Token>();
            var existingLocalTerms = termsBatch.Where(term => tokens.Any(t => t.Term == term.TermText && t.TermType == term.TermType));
            
            var newLocalTerms = new List<Term>();
            
            foreach (var token in tokens)
            {
                if (!termsBatch.Any(t => t.TermText == token.Term && t.TermType == token.TermType))
                {
                    var newTerm = new Term
                    {
                        TermText = token.Term,
                        TermType = token.TermType,
                    };
                    
                    newLocalTerms.Add(newTerm);
                }
                
                tokenLookup.Add((token.Term, token.TermType), token);
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

                if (tokenLookup[lookupKey].TermType.NotNgram())
                {
                    var termVector = new TermVector
                    {
                        Term = indexingTerm,
                        Movie = movie,
                        FieldType = tokenLookup[lookupKey].FieldType,
                    };
                    
                    termVectorsBatch.Add(termVector);
                }
                
                termIndicesBatch.Add(termIndex);
            }
            
            termsBatch.AddRange(newLocalTerms);
            moviesBatch.Add(movie);
            
            Interlocked.Increment(ref _totalCount);
        }
        
        await scopedDatabase.Store<Term>().BulkAddAsync(termsBatch, ct);
        await scopedDatabase.Store<Movie>().BulkAddAsync(moviesBatch, ct);
        await scopedDatabase.Store<TermIndex>().BulkAddAsync(termIndicesBatch, ct);
        await scopedDatabase.Store<TermVector>().BulkAddAsync(termVectorsBatch, ct);
    }

    private async Task PopulateTfIdfScoreAsync(CancellationToken cancellationToken)
    {
        var includeProperties = new[]
        {
            $"{nameof(TermIndex.Term)}"
        };
        
        var vectorQueryable = database.Store<TermVector>()
            .AsQueryable()
            .OrderBy(vector => vector.TermId)
            .ThenBy(vector => vector.MovieId);
        
        var moviesCount = await database.Store<Movie>().CountAsync(cancellationToken: cancellationToken);
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount, 
            CancellationToken = cancellationToken
        };
        
        const int batchSize = 1000;
        await BatchProcessor.BatchAsync(vectorQueryable, batchSize, async batch =>
        {
            using var taskScope = scopeFactory.CreateScope();
            var scopedDatabase = taskScope.ServiceProvider.GetRequiredService<IDatabase<MovieDbContext>>();
            var batchKeys = batch.Select(item => new { item.TermId, item.MovieId }).ToList();
            
            var movieIds = batchKeys.Select(k => k.MovieId).ToList();
            var termIds = batchKeys.Select(k => k.TermId).ToList();
            
            var termIndices = await scopedDatabase.Store<TermIndex>()
                .ListAsync(
                    index => movieIds.Contains(index.MovieId) && termIds.Contains(index.TermId),
                    includeProperties: includeProperties,
                    cancellationToken: cancellationToken);
            
            var termsCountLookup = await scopedDatabase.Store<TermIndex>()
                .AsQueryable()
                .GroupBy(ti => ti.TermId)
                .Select(g => new { TermId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TermId, x => x.Count, cancellationToken);

            var termsPerDocumentCountLookup = await scopedDatabase.Store<TermIndex>()
                .AsQueryable()
                .Include(ti => ti.Term)
                .Where(ti => ti.Term.TermType == TermType.CharactersNgram || ti.Term.TermType == TermType.WordNgram)
                .GroupBy(ti => ti.MovieId)
                .Select(g => new { MovieId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MovieId, x => x.Count, cancellationToken);
            
            Parallel.ForEach(batch, parallelOptions, async termVector =>
            {
                var index = termIndices.First(ti => ti.MovieId == termVector.MovieId && ti.TermId == termVector.TermId);
                
                int termsCountPerDocument = termsPerDocumentCountLookup.GetValueOrDefault(index.MovieId);
                int numberOfDocumentsContainingTerm = termsCountLookup.GetValueOrDefault(index.TermId);
                
                double tf = termsCountPerDocument > 0 ? index.TermFrequency / (double)termsCountPerDocument : 0;
                double idf = numberOfDocumentsContainingTerm > 0 ? Math.Log(moviesCount / (double)numberOfDocumentsContainingTerm) : 0;
                double tfIdfScore = tf * idf;
                
                termVector.Score = tfIdfScore;
            });
            
            await scopedDatabase.Store<TermVector>().BulkUpdateAsync(batch, cancellationToken);
        });
    }
}