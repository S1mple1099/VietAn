namespace Monitoring.Application.DTOs.Common;

/// <summary>
/// Kết quả phân trang
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu trong danh sách</typeparam>
public record PagedResult<T>
{
    /// <summary>
    /// Danh sách dữ liệu của trang hiện tại
    /// </summary>
    public IEnumerable<T> Items { get; init; } = [];

    /// <summary>
    /// Tổng số bản ghi
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Số trang hiện tại (bắt đầu từ 1)
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Số bản ghi mỗi trang
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Tổng số trang
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Có trang trước không
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Có trang sau không
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}
