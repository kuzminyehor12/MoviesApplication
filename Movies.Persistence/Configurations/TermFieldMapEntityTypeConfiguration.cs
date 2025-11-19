using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class TermFieldMapEntityTypeConfiguration : IEntityTypeConfiguration<TermFieldMap>
{
    public void Configure(EntityTypeBuilder<TermFieldMap> builder)
    {
        builder
            .Property(fieldMap => fieldMap.Id)
            .UseIdentityColumn();
        
        builder
            .HasIndex(t => t.FieldType)
            .HasDatabaseName("IX_TermFieldMap_FieldMap");
    }
}