using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using V4H.Domain.Entities;

namespace V4H.Infrastructure.Persistence.Configurations;

public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
{
    public void Configure(EntityTypeBuilder<StatusHistory> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Notes).HasMaxLength(1000);
        builder.HasOne(s => s.ChangedBy).WithMany()
            .HasForeignKey(s => s.ChangedById).OnDelete(DeleteBehavior.Restrict);
    }
}
