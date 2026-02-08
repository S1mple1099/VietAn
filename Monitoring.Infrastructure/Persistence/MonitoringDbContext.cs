namespace Monitoring.Infrastructure.Persistence;

using DomainRole = Monitoring.Domain.Entities.Role;

/// <summary>
/// EF Core DbContext for Monitoring System
/// Uses Code-First approach with Fluent API configurations
/// </summary>
public class MonitoringDbContext : DbContext
{
    public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<DomainRole> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<TagHistory> TagHistories { get; set; } = null!;
    public DbSet<LoginLog> LoginLogs { get; set; } = null!;
    public DbSet<EventLog> EventLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MonitoringDbContext).Assembly);
    }
}
