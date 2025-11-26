using Microsoft.EntityFrameworkCore;
using Movies.Core.Entities;

namespace Movies.Persistence.Infrastructure;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }
    
    public DbSet<Term> Terms { get; set; }
    
    public DbSet<TermIndex> TermIndex { get; set; }
    
    public DbSet<TermVector> TermVectors { get; set; }
    
    public DbSet<TermFieldMap> TermFieldMaps { get; set; }
    
    public DbSet<MovieEmbedding> MovieEmbeddings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("vector");
        
        modelBuilder.HasDbFunction(typeof(SqlFunctions).GetMethod(nameof(SqlFunctions.CalculateJaccard)))
            .HasName("calculate_jaccard");
        
        modelBuilder.HasDbFunction(typeof(SqlFunctions).GetMethod(nameof(SqlFunctions.CalculateDice)))
            .HasName("calculate_dice");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);
    }
}