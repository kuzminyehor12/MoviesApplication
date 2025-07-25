using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }
    
    public DbSet<Term> Terms { get; set; }
    
    public DbSet<TermIndex> TermIndex { get; set; }
    
    public DbSet<TermVector> TermVectors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);
    }
}