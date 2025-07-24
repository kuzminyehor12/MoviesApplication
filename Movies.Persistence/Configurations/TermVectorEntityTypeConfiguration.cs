using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class TermVectorEntityTypeConfiguration : IEntityTypeConfiguration<TermVector>
{
    public void Configure(EntityTypeBuilder<TermVector> builder)
    {
        builder.HasKey(termVector => new { termVector.TermId, termVector.MovieId });
    }
}