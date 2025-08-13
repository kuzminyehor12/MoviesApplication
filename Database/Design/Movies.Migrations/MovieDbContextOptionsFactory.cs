using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Movies.Persistence.Infrastructure;

namespace Movies.Migrations;

public static class MovieDbContextOptionsFactory
{
    public static DbContextOptions<MovieDbContext> CreateDbContextOptions(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"Connection string '{connectionString}' not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<MovieDbContext>();
        optionsBuilder.UseNpgsql(connectionString, options => options.MigrationsAssembly(typeof(MovieDbContextFactory).Assembly.FullName));
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

        return optionsBuilder.Options;
    }
}