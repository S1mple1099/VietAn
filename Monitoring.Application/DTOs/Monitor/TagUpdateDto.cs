namespace Monitoring.Application.DTOs.Monitor;

/// <summary>
/// DTO for real-time tag updates sent via SignalR
/// </summary>
public record TagUpdateDto(
    int TagId,
    string TagName,
    int PumpId,
    DateTime Timestamp,
    object? Value,
    string Quality);
