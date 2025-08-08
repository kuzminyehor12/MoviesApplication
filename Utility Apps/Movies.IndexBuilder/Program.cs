using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Configuration;
using Movies.IndexBuilder.Configurations;
using Movies.IndexBuilder.Startup;
using Movies.Migrations;
using Movies.Persistence.Infrastructure;
using Movies.Search;

namespace Movies.IndexBuilder;

static class Program
{
    static IConfiguration Configuration => MovieConfigurationManager.Configuration;
    
    static async Task Main(string[] args)
    {
        // TODO: Create an utility which takes a data from directory and puts it into an inverted index
        // Index should represent this structure:
        // -----------------------------------------------------------
        // | keyword_id | keyword_word | inversed_document_frequency |
        // -----------------------------------------------------------
        // ------------------------------------------
        // | keyword_id | movie_id | term_frequency |
        // ------------------------------------------
        
        // TODO: Use preprocessing for data:
        // - normalize data in ~/Data folder(using jupyter notebook maybe)
        // - do lemmatizing/stemming
        // - remove stop words
        
        // TODO: Keep infrastructure up-to-date(do it after infrastructure will be defined) 
        // - Run container instance of database
        // - During reindexing we have to consider having two instances(one to be available and other to be re-indexed based on updated data)
        // - Consider infrastructure clean up afterwards
        // - Roll back to available instance if indexing error happened
        
        // Create PoC for two cases: MongoDb and Postgres
        // Use benchmarks to determine which index is faster
        
        // Index original title to find exact match
        // If exact match not found try fuzzy search
        // use poster_path with https://image.tmdb.org/t/p/original/
        // use profile_path with https://image.tmdb.org/t/p/w200/
        
        // How to compute score:
        // * bonus for exact match
        // * fuzzy search
        // * bonus for position
        // * scoring threshold
        // * set weights per movie property
        // * TF-IDF vs N-gram?
        
        // Algorithm
        // 1. Add a document(movie)
        // 2. Extract terms/n-grams considering type from the document. IMPORTANT! Separate n-grams from terms for TF-IDF vectorization 
        // 3. Normalize them 
        // 4. Put into terms table
        // 5. Add a relationship between document and a term with a frequency and a position
        // 6. After all document added compute TF-IDF and place it into document_vectors table

        var serviceProvider = ConfigureServices();
        
        var indexBuilderRunner = serviceProvider.GetRequiredService<IndexBuilderRunner>();

        await indexBuilderRunner.StartAsync();
    }

    private static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddConfigurations();
        
        serviceCollection.AddDatabase();

        serviceCollection.AddScoped<ITokenExtractor, TokenExtractor>();
        
        serviceCollection.AddSingleton<IndexBuilderRunner>();
        
        return serviceCollection.BuildServiceProvider();
    }

    private static void AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<MovieDbContext>(_ => GetMovieDbContext(Configuration.GetConnectionString("MovieDbConnection")));
        
        services.AddScoped(typeof(IDatabase<>), typeof(Database<>));
        
        services.AddSingleton<IDataStoreFactory, DataStoreFactory>();
    }

    private static void AddConfigurations(this IServiceCollection services)
    {
        services.AddSingleton(new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        });
        
        services.Configure<IndexingDataConfiguration>(Configuration.GetSection("IndexingDataConfiguration"));
    }
    
    private static MovieDbContext GetMovieDbContext(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"Connection string '{connectionString}' not found.");
        }
        
        var options = MovieDbContextOptionsFactory.CreateDbContextOptions(connectionString);
        return new MovieDbContext(options);
    }
}