namespace Monitoring.Application.DTOs.History;

public record GetHistoryRequest
{
    public string? EventType { get; init; } = "all";
    public string? SearchText { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool IncludeLoginLogs { get; init; } = true;
    
    /// <summary>
    /// Số trang (bắt đầu từ 1), mặc định là 1
    /// </summary>
    public int Page { get; init; } = 1;
    
    /// <summary>
    /// Số bản ghi mỗi trang, mặc định là 15
    /// </summary>
    public int PageSize { get; init; } = 15;
}
