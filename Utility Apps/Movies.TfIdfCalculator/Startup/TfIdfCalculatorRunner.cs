using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movies.Core.Entities;
using Movies.Core.Enums;
using Movies.Persistence.Batching;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using Movies.Search.Models;

namespace Movies.TfIdfCalculator.Startup;

public class TfIdfCalculatorRunner(
    IDatabase<MovieDbContext> database,
    IServiceScopeFactory scopeFactory)
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await PopulateTfIdfScoreAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(-1);
        }
    }
    
    private async Task PopulateTfIdfScoreAsync(CancellationToken cancellationToken)
    {
        var includeProperties = new[]
        {
            $"{nameof(TermIndex.Term)}"
        };
        
        var vectorQueryable = database.Store<TermVector>()
            .AsQueryable()
            .Where(vector => vector.Score == 0)
            .OrderBy(vector => vector.TermId)
            .ThenBy(vector => vector.MovieId);
        
        var moviesCount = await database.Store<Movie>().CountAsync(cancellationToken: cancellationToken);
        
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount, 
            CancellationToken = cancellationToken
        };
        
        var movies = await database
            .Store<Movie>()
            .ListAsync(cancellationToken: cancellationToken);

        var tokensLookup = new ConcurrentDictionary<int, IReadOnlySet<Token>>();

        Parallel.ForEach(movies, parallelOptions, movie =>
        {
            var taskScope = scopeFactory.CreateScope();
            var extractor = taskScope.ServiceProvider.GetRequiredService<ITokenExtractor>();
            tokensLookup.TryAdd(movie.Id, extractor.Extract(movie.ToString(), FieldType.Complex));
        });
        
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
                .Where(ti => ti.Term.TermType == TermType.WholeWord || ti.Term.TermType == TermType.WholeString)
                .GroupBy(ti => ti.Term.TermText)
                .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
            
            Parallel.ForEach(batch, parallelOptions, async termVector =>
            {
                var index = termIndices.First(ti =>
                    ti.MovieId == termVector.MovieId && ti.TermId == termVector.TermId);
                
                int numberOfDocumentsContainingTerm = termsCountLookup.GetValueOrDefault(index.Term.TermText);
                var tokens = tokensLookup[index.MovieId];
                var term = tokens.FirstOrDefault(token => token.Term == index.Term.TermText);
                
                if (term is null)
                {
                    Console.WriteLine($"Term {index.Term.TermText} was not presented in tokens list.");
                }
                
                double tf = tokens.Count > 0 && term != null ? term.Frequency / (double)tokens.Count : 0;
                double idf = numberOfDocumentsContainingTerm > 0
                    ? Math.Log(moviesCount / (double)numberOfDocumentsContainingTerm)
                    : 0;
                
                double tfIdfScore = tf * idf;
                termVector.Score = tfIdfScore;
            });
            
            await scopedDatabase.Store<TermVector>().BulkUpdateAsync(batch, cancellationToken);
        });
    }
}