
namespace Monitoring.Application.Interfaces;

/// <summary>
/// Interface dịch vụ quản lý lịch sử sự kiện và đăng nhập
/// </summary>
public interface IHistoryService
{
    /// <summary>
    /// Lấy lịch sử sự kiện và đăng nhập với các điều kiện lọc (có phân trang)
    /// </summary>
    /// <param name="eventType">Loại sự kiện (login, error, warn, ok) hoặc null để lấy tất cả</param>
    /// <param name="searchText">Từ khóa tìm kiếm trong mô tả</param>
    /// <param name="fromDate">Ngày bắt đầu, null sẽ lấy ngày hôm nay</param>
    /// <param name="toDate">Ngày kết thúc, null sẽ lấy ngày hôm nay</param>
    /// <param name="includeLoginLogs">Có bao gồm log đăng nhập không</param>
    /// <param name="page">Số trang (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số bản ghi mỗi trang</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân trang lịch sử sự kiện</returns>
    Task<PagedResult<HistoryRowDto>> GetHistoryAsync(
        string? eventType,
        string? searchText,
        DateTime? fromDate,
        DateTime? toDate,
        bool includeLoginLogs = false,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách các loại sự kiện có trong hệ thống
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách loại sự kiện</returns>
    Task<IEnumerable<EventTypeOptionDto>> GetEventTypesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Xuất lịch sử sự kiện ra file Excel
    /// </summary>
    /// <param name="eventType">Loại sự kiện hoặc null để lấy tất cả</param>
    /// <param name="searchText">Từ khóa tìm kiếm</param>
    /// <param name="fromDate">Ngày bắt đầu</param>
    /// <param name="toDate">Ngày kết thúc</param>
    /// <param name="includeLoginLogs">Có bao gồm log đăng nhập không</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Mảng byte chứa nội dung file Excel</returns>
    Task<byte[]> ExportHistoryAsync(string? eventType, string? searchText, DateTime? fromDate, DateTime? toDate, bool includeLoginLogs = true, CancellationToken cancellationToken = default);
}
