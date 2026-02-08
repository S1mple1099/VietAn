namespace Monitoring.Application.Interfaces;

using Monitoring.Application.DTOs.Monitor;

/// <summary>
/// Interface phát sóng cập nhật real-time đến các client
/// Implementation sẽ ở Host layer (SignalR)
/// </summary>
public interface IRealtimeBroadcaster
{
    /// <summary>
    /// Phát sóng cập nhật tag đến tất cả client đang kết nối
    /// </summary>
    /// <param name="tagUpdate">Thông tin cập nhật tag</param>
    /// <param name="cancellationToken">Token hủy</param>
    Task BroadcastTagUpdateAsync(TagUpdateDto tagUpdate, CancellationToken cancellationToken = default);
}
