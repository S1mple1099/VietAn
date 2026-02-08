using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monitoring.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        // Indexes for common queries
        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.PumpId);
        builder.HasIndex(t => new { t.PumpId, t.IsActive });

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Unit)
            .HasMaxLength(50);

        builder.Property(t => t.DataType)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Double");

        builder.Property(t => t.PumpId)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(t => t.TagHistories)
            .WithOne(th => th.Tag)
            .HasForeignKey(th => th.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
