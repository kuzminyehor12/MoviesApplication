using Microsoft.EntityFrameworkCore;
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

        return optionsBuilder.Options;
    }
}