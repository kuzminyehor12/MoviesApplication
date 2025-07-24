using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class MovieEntityTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder
            .Property(movie => movie.Id)
            .UseIdentityColumn();
        
        builder
            .HasIndex(movie => movie.Title)
            .IsUnique();
        
        builder
            .Property(movie => movie.CastMembers)
            .HasColumnName("cast")
            .HasColumnType("jsonb");
        
        builder
            .Property(movie => movie.CrewMembers)
            .HasColumnName("crew")
            .HasColumnType("jsonb");
        
        builder
            .Property(movie => movie.Keywords)
            .HasColumnName("keywords")
            .HasColumnType("jsonb");
        
        builder
            .Property(movie => movie.Genres)
            .HasColumnName("genres")
            .HasColumnType("jsonb");
    }
}