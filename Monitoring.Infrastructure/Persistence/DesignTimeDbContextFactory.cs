namespace Monitoring.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Factory for creating DbContext at design time (for migrations)
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MonitoringDbContext>
{
    public MonitoringDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MonitoringDbContext>();
        
        // Use SQLite for design time (migrations)
        // Connection string will be overridden at runtime
        optionsBuilder.UseSqlite("Data Source=monitoring.db");

        return new MonitoringDbContext(optionsBuilder.Options);
    }
}
