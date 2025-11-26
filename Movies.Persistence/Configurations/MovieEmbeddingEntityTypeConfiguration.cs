using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;
using Pgvector;

namespace Movies.Persistence.Configurations;

public class MovieEmbeddingEntityTypeConfiguration : IEntityTypeConfiguration<MovieEmbedding>
{
    public void Configure(EntityTypeBuilder<MovieEmbedding> builder)
    {
        builder
            .Property(movie => movie.Id)
            .UseIdentityColumn();
        
        builder.Property(me => me.Vector)
            .HasConversion(e => new Vector(e), v => v.ToArray())
            .HasColumnType("vector(1024)");
        
        builder
            .HasOne(e => e.Movie)
            .WithOne(m => m.Embedding)
            .HasForeignKey<MovieEmbedding>(m => m.MovieId);
    }
}