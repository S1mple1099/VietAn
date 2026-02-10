namespace Monitoring.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monitoring.Infrastructure.Persistence;

/// <summary>
/// Extension methods for configuring Infrastructure services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services including EF Core DbContext
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Lấy connection string từ configuration (env vars override appsettings theo thứ tự load)
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // SQLite provider (Data Source=...)
        services.AddDbContext<MonitoringDbContext>(options =>
        {
            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.MigrationsAssembly(typeof(MonitoringDbContext).Assembly.FullName);
            });

            // Chỉ bật trong Development để tránh lộ dữ liệu nhạy cảm ở Production
            if (string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        return services;
    }
}
