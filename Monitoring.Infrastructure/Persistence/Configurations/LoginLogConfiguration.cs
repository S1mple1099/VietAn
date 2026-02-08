using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Monitoring.Infrastructure.Persistence.Configurations;

public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
{
    public void Configure(EntityTypeBuilder<LoginLog> builder)
    {
        builder.ToTable("LoginLogs");

        builder.HasKey(ll => ll.Id);

        // Indexes for common queries
        builder.HasIndex(ll => ll.UserId);
        builder.HasIndex(ll => ll.Timestamp);
        builder.HasIndex(ll => new { ll.Username, ll.Timestamp });
        builder.HasIndex(ll => ll.IsSuccess);

        builder.Property(ll => ll.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ll => ll.IpAddress)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ll => ll.UserAgent)
            .HasMaxLength(500);

        builder.Property(ll => ll.IsSuccess)
            .IsRequired();

        builder.Property(ll => ll.FailureReason)
            .HasMaxLength(500);

        builder.Property(ll => ll.Timestamp)
            .IsRequired();

        // Relationship (nullable - user might be deleted)
        builder.HasOne(ll => ll.User)
            .WithMany(u => u.LoginLogs)
            .HasForeignKey(ll => ll.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
