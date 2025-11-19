using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Movies.Configuration;
using Movies.Configuration.Database;
using Movies.Persistence.Infrastructure;

namespace Movies.Migrations;

public class MovieDbContextFactory : IDesignTimeDbContextFactory<MovieDbContext>
{
    private readonly IEnvironmentConnectionStringFactory _connectionStringFactory;

    public MovieDbContextFactory()
    {
        _connectionStringFactory = new EnvironmentConnectionStringFactory(MovieConfigurationManager.Configuration);
    }
    
    public MovieDbContextFactory(IEnvironmentConnectionStringFactory connectionStringFactory)
    {
        _connectionStringFactory = connectionStringFactory;
    }

    public MovieDbContext CreateDbContext(string[] args)
    {
        var connectionString = _connectionStringFactory.GetInactiveDatabaseEnvironment();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string not found.");
        }
        
        var options = MovieDbContextOptionsFactory.CreateDbContextOptions(connectionString);

        return new MovieDbContext(options);
    }
}