
namespace Monitoring.Application.Interfaces;

/// <summary>
/// Interface dịch vụ quản lý dữ liệu giám sát máy bơm
/// </summary>
public interface IMonitorService
{
    /// <summary>
    /// Lấy dữ liệu giám sát theo máy bơm và khoảng thời gian (có phân trang)
    /// </summary>
    /// <param name="pumpId">ID của máy bơm (ví dụ: pump1, pump2, pump3)</param>
    /// <param name="fromDate">Ngày bắt đầu</param>
    /// <param name="toDate">Ngày kết thúc</param>
    /// <param name="page">Số trang (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số bản ghi mỗi trang</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân trang dữ liệu giám sát được nhóm theo phút</returns>
    Task<PagedResult<MonitorRowDto>> GetMonitorDataAsync(
        string pumpId,
        DateTime fromDate,
        DateTime toDate,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách tất cả các máy bơm
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách máy bơm</returns>
    Task<IEnumerable<PumpOptionDto>> GetPumpsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Xuất dữ liệu giám sát ra file Excel
    /// </summary>
    /// <param name="pumpId">ID của máy bơm</param>
    /// <param name="fromDate">Ngày bắt đầu</param>
    /// <param name="toDate">Ngày kết thúc</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Mảng byte chứa nội dung file Excel</returns>
    Task<byte[]> ExportMonitorDataAsync(string pumpId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
