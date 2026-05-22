using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class TeleconsultoriaConfiguration : IEntityTypeConfiguration<Teleconsultoria>
{
    public void Configure(EntityTypeBuilder<Teleconsultoria> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.PatientName).HasMaxLength(300).IsRequired();
        builder.Property(t => t.DiagnosticHypothesis).HasMaxLength(2000).IsRequired();
        builder.Property(t => t.ClinicalHistory).HasMaxLength(5000).IsRequired();
        builder.HasOne(t => t.Requester).WithMany()
            .HasForeignKey(t => t.RequesterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(t => t.Documents).WithOne(d => d.Teleconsultoria)
            .HasForeignKey(d => d.TeleconsultoriaId);
        builder.HasMany(t => t.Opinions).WithOne(o => o.Teleconsultoria)
            .HasForeignKey(o => o.TeleconsultoriaId);
        builder.HasMany(t => t.StatusHistories).WithOne(s => s.Teleconsultoria)
            .HasForeignKey(s => s.TeleconsultoriaId);
    }
}
