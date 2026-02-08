using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monitoring.Infrastructure.Persistence.Configurations;

public class TagHistoryConfiguration : IEntityTypeConfiguration<TagHistory>
{
    public void Configure(EntityTypeBuilder<TagHistory> builder)
    {
        builder.ToTable("TagHistories");

        builder.HasKey(th => th.Id);

        // Composite indexes for performance (common query patterns)
        builder.HasIndex(th => new { th.TagId, th.Timestamp });
        builder.HasIndex(th => new { th.PumpId, th.Timestamp });
        builder.HasIndex(th => th.Timestamp);

        builder.Property(th => th.TagId)
            .IsRequired();

        builder.Property(th => th.PumpId)
            .IsRequired();

        builder.Property(th => th.Timestamp)
            .IsRequired();

        builder.Property(th => th.Quality)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Good");

        // Value columns (nullable - only one will be used based on Tag.DataType)
        builder.Property(th => th.ValueDouble);
        builder.Property(th => th.ValueInt);
        builder.Property(th => th.ValueBool);
        builder.Property(th => th.ValueString)
            .HasMaxLength(500);
    }
}
