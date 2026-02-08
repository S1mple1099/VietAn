namespace Monitoring.Application.DTOs.Monitor;

public record GetMonitorDataRequest
{
    public string? PumpId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    
    /// <summary>
    /// Số trang (bắt đầu từ 1), mặc định là 1
    /// </summary>
    public int Page { get; init; } = 1;
    
    /// <summary>
    /// Số bản ghi mỗi trang, mặc định là 15
    /// </summary>
    public int PageSize { get; init; } = 15;
}
