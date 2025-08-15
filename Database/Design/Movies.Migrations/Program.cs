using Microsoft.EntityFrameworkCore;
using Movies.Persistence.Infrastructure;

namespace Movies.Migrations;

class Program
{
    static void Main(string[] args)
    {
        var factory = new MovieDbContextFactory();
        MovieDbContext dbContext = factory.CreateDbContext(args);
        
        Console.WriteLine("Cleaning up database...");
        dbContext.Database.EnsureDeleted();
        
        Console.WriteLine("Creating database...");
        dbContext.Database.Migrate();
    }
}