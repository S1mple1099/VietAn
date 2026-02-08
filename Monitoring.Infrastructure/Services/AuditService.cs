
namespace Monitoring.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly MonitoringDbContext _context;

    public AuditService(MonitoringDbContext context)
    {
        _context = context;
    }

    public async Task LogLoginAsync(Guid userId, string username, string ipAddress, string userAgent, bool isSuccess, string? failureReason = null, CancellationToken cancellationToken = default)
    {
        var log = new LoginLog
        {
            UserId = userId != Guid.Empty ? userId : null,
            Username = username,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            FailureReason = failureReason,
            Timestamp = DateTime.UtcNow
        };

        _context.LoginLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task LogEventAsync(string eventType, string device, string? account, string description, string? additionalData = null, CancellationToken cancellationToken = default)
    {
        var log = new EventLog
        {
            EventType = eventType,
            Device = device,
            Account = account,
            Description = description,
            AdditionalData = additionalData,
            Timestamp = DateTime.UtcNow
        };

        _context.EventLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
