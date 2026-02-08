namespace Monitoring.Application.DTOs.History;

public record HistoryRowDto(
    string Id,
    string Time,
    string Device,
    string Account,
    string Type,
    string Description,
    string? ErrorCode = null,
    string? ProcessingTime = null);
