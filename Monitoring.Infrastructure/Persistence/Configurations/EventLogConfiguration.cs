using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monitoring.Infrastructure.Persistence.Configurations;

public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
{
    public void Configure(EntityTypeBuilder<EventLog> builder)
    {
        builder.ToTable("EventLogs");

        builder.HasKey(el => el.Id);

        // Indexes for common queries
        builder.HasIndex(el => el.EventType);
        builder.HasIndex(el => el.Timestamp);
        builder.HasIndex(el => new { el.EventType, el.Timestamp });
        builder.HasIndex(el => el.Device);

        builder.Property(el => el.EventType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(el => el.Device)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(el => el.Account)
            .HasMaxLength(100);

        builder.Property(el => el.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(el => el.AdditionalData)
            .HasMaxLength(2000);

        builder.Property(el => el.ErrorCode)
            .HasMaxLength(50);

        builder.Property(el => el.ProcessingTimeSeconds);

        builder.Property(el => el.Timestamp)
            .IsRequired();
    }
}
