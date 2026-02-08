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
        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Register DbContext with SQLite provider
        services.AddDbContext<MonitoringDbContext>(options =>
        {
            options.UseSqlite(connectionString, sqliteOptions =>
            {
                // SQLite-specific options
                sqliteOptions.MigrationsAssembly(typeof(MonitoringDbContext).Assembly.FullName);
            });

            // Enable sensitive data logging in development only
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        return services;
    }
}
