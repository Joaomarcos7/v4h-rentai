using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<TeleconsultoriaDocument>
{
    public void Configure(EntityTypeBuilder<TeleconsultoriaDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FileName).HasMaxLength(500).IsRequired();
        builder.Property(d => d.StoredPath).HasMaxLength(1000).IsRequired();
        builder.Property(d => d.ValidationScore).HasPrecision(5, 4);
        builder.Property(d => d.ValidationThreshold).HasPrecision(5, 4);
        builder.Property(d => d.ValidationProvider).HasMaxLength(100);
    }
}
