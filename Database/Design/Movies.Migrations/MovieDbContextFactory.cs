using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Movies.Configuration;
using Movies.Configuration.Database;
using Movies.Persistence.Infrastructure;

namespace Movies.Migrations;

public class MovieDbContextFactory(IEnvironmentConnectionStringFactory connectionStringFactory) : IDesignTimeDbContextFactory<MovieDbContext>
{
    public MovieDbContext CreateDbContext(string[] args)
    {
        var connectionString = connectionStringFactory.GetInactiveDatabaseEnvironment();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string not found.");
        }
        
        var options = MovieDbContextOptionsFactory.CreateDbContextOptions(connectionString);

        return new MovieDbContext(options);
    }
}