using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class TermVectorEntityTypeConfiguration : IEntityTypeConfiguration<TermVector>
{
    public void Configure(EntityTypeBuilder<TermVector> builder)
    {
        builder.Ignore(termVector => termVector.Id);
        builder.HasKey(termVector => new { termVector.TermId, termVector.MovieId });
        builder
            .HasIndex(ti => new { ti.MovieId, ti.TermId })
            .HasDatabaseName("IX_TermVectors_MovieTerm");
    }
}