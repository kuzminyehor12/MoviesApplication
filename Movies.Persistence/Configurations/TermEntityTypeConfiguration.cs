using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class TermEntityTypeConfiguration  : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder
            .Property(term => term.Id)
            .UseIdentityByDefaultColumn();
        
        builder.HasIndex(term => term.TermText);
    }
}