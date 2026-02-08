namespace Monitoring.Domain.Entities;

/// <summary>
/// Event log entity for system events and audit trail
/// </summary>
public class EventLog
{
    public long Id { get; set; }
    public string EventType { get; set; } = string.Empty; // login, error, warn, ok
    public string Device { get; set; } = string.Empty; // PLC, Bom 1, Bom 2, etc.
    public string? Account { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ErrorCode { get; set; } // Mã lỗi (nếu có)
    public int? ProcessingTimeSeconds { get; set; } // Thời gian xử lý (giây)
    public string? AdditionalData { get; set; } // JSON for additional context
}
