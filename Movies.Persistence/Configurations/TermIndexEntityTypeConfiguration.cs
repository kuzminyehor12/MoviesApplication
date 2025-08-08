using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class TermIndexEntityTypeConfiguration : IEntityTypeConfiguration<TermIndex>
{
    public void Configure(EntityTypeBuilder<TermIndex> builder)
    {
        builder.Ignore(termIndex => termIndex.Id);
        builder.HasKey(termIndex => new { termIndex.TermId, termIndex.MovieId });
    }
}