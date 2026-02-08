namespace Monitoring.Domain.Entities;

/// <summary>
/// Login log entity for audit trail
/// </summary>
public class LoginLog
{
    public long Id { get; set; }
    public Guid? UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? User { get; set; }
}
