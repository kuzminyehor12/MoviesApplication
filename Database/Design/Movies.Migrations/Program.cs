using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Configuration;
using Movies.Configuration.Database;
using Movies.Persistence.Infrastructure;

namespace Movies.Migrations;

class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        
        var factory = serviceProvider.GetRequiredService<MovieDbContextFactory>();
        MovieDbContext dbContext = factory.CreateDbContext(args);
        
        Console.WriteLine("Cleaning up database...");
        dbContext.Database.EnsureDeleted();
        
        Console.WriteLine("Creating database...");
        dbContext.Database.Migrate();
    }
    
    private static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton(MovieConfigurationManager.Configuration);
        
        serviceCollection.AddScoped<IEnvironmentConnectionStringFactory, EnvironmentConnectionStringFactory>();
        
        serviceCollection.AddScoped<MovieDbContextFactory>();
        
        return serviceCollection.BuildServiceProvider();
    }
}