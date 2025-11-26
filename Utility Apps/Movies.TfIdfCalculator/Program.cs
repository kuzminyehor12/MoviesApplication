using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Configuration;
using Movies.Configuration.Database;
using Movies.Migrations;
using Movies.Persistence.Infrastructure;
using Movies.Search;
using Movies.TfIdfCalculator.Startup;

namespace Movies.TfIdfCalculator;

static class Program
{
    static IConfiguration Configuration => MovieConfigurationManager.Configuration;
    
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        
        var indexBuilderRunner = serviceProvider.GetRequiredService<TfIdfCalculatorRunner>();
        
        Console.WriteLine("Application has started. Ctrl-C to end");
        
        CancellationTokenSource cts = new CancellationTokenSource();
        
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Canceling...");
            cts.Cancel();
            eventArgs.Cancel = true;
        };

        await indexBuilderRunner.StartAsync(cts.Token);
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddConfigurations();
        
        serviceCollection.AddDatabase();

        serviceCollection.AddScoped<ITokenExtractor, TokenExtractor>();
        
        serviceCollection.AddSingleton<TfIdfCalculatorRunner>();
        
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