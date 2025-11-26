using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Movies.Configuration;
using Movies.Configuration.Configurations;
using Movies.Configuration.Database;
using Movies.Migrations;
using Movies.MovieEmbeddingBuilder.Startup;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using OllamaSharp;

namespace Movies.MovieEmbeddingBuilder;

static class Program
{
    static IConfiguration Configuration => MovieConfigurationManager.Configuration;
    
    
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        
        var runner = serviceProvider.GetRequiredService<MovieEmbeddingBuilderRunner>();
        
        Console.WriteLine("Application has started. Ctrl-C to end");
        
        CancellationTokenSource cts = new CancellationTokenSource();
        
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Canceling...");
            cts.Cancel();
            eventArgs.Cancel = true;
        };

        await runner.StartAsync(cts.Token);
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddConfigurations();
        
        serviceCollection.AddDatabase();
        
        serviceCollection.Configure<OllamaApiClientSettings>(Configuration.GetSection("OllamaApiClientSettings"));
        
        serviceCollection.AddScoped<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var ollamaSettings = sp.GetRequiredService<IOptions<OllamaApiClientSettings>>().Value;
            return new OllamaApiClient(new Uri(ollamaSettings.Url), ollamaSettings.Model);
        });
        
        serviceCollection.AddScoped<IVectorGenerator, VectorGenerator>();
        
        serviceCollection.AddSingleton<MovieEmbeddingBuilderRunner>();
        
        return serviceCollection.BuildServiceProvider();
    }

    private static void AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<IEnvironmentConnectionStringFactory, EnvironmentConnectionStringFactory>();
        
        services.AddScoped<MovieDbContext>(sp =>
        {
            var connectionStringFactory = sp.GetRequiredService<IEnvironmentConnectionStringFactory>();
            return GetMovieDbContext(connectionStringFactory.GetInactiveDatabaseEnvironment());
        });
        
        services.AddScoped(typeof(IDataStoreFactory<>), typeof(DataStoreFactory<>));
        
        services.AddScoped(typeof(IDatabase<>), typeof(Database<>));
    }

    private static void AddConfigurations(this IServiceCollection services)
    {
        services.AddSingleton(Configuration);
    }

    private static MovieDbContext GetMovieDbContext(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"Connection string '{connectionString}' not found.");
        }

        var options = MovieDbContextOptionsFactory.CreateDbContextOptions(connectionString);
        var factory = new PooledDbContextFactory<MovieDbContext>(options);

        return factory.CreateDbContext();
    }
}