
namespace Monitoring.Application.Interfaces;

/// <summary>
/// Interface dịch vụ cache và truy xuất giá trị tag real-time
/// </summary>
public interface ITagCacheService
{
    /// <summary>
    /// Cập nhật giá trị tag vào cache
    /// </summary>
    /// <param name="tagUpdate">Thông tin cập nhật tag</param>
    /// <param name="cancellationToken">Token hủy</param>
    Task UpdateTagAsync(TagUpdateDto tagUpdate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy các tag mới nhất của một máy bơm
    /// </summary>
    /// <param name="pumpId">ID của máy bơm (1, 2, hoặc 3)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách tag mới nhất của máy bơm</returns>
    Task<IEnumerable<TagUpdateDto>> GetLatestTagsAsync(int pumpId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy tất cả các tag mới nhất của tất cả máy bơm
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách tất cả tag mới nhất</returns>
    Task<IEnumerable<TagUpdateDto>> GetAllLatestTagsAsync(CancellationToken cancellationToken = default);
}
