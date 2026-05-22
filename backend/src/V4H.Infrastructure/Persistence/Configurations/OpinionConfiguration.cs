using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class OpinionConfiguration : IEntityTypeConfiguration<Opinion>
{
    public void Configure(EntityTypeBuilder<Opinion> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Content).HasMaxLength(10000).IsRequired();
        builder.HasOne(o => o.Specialist).WithMany()
            .HasForeignKey(o => o.SpecialistId).OnDelete(DeleteBehavior.Restrict);
    }
}
