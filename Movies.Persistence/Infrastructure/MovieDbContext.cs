using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public class MovieDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    
    public DbSet<Term> Terms { get; set; }
    
    public DbSet<TermIndex> TermIndex { get; set; }
    
    public DbSet<TermVector> TermVectors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}