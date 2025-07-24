using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Movies.Configuration;
using Movies.Persistence.Infrastructure;

namespace Movies.Migrations;

public class MovieDbContextFactory : IDesignTimeDbContextFactory<MovieDbContext>
{
    private const string ConnectionStringName = "MovieDbConnection";
    
    private IConfiguration Configuration => MovieConfigurationManager.Configuration;
    
    public MovieDbContext CreateDbContext(string[] args)
    {
        var connectionString = Configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{ConnectionStringName}' not found.");
        }
        
        var options = MovieDbContextOptionsFactory.CreateDbContextOptions(connectionString);

        return new MovieDbContext(options);
    }
}