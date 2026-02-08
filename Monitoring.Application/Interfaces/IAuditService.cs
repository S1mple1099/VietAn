namespace Monitoring.Application.Interfaces;

/// <summary>
/// Interface dịch vụ ghi log kiểm toán và sự kiện
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Ghi log đăng nhập vào hệ thống
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="ipAddress">Địa chỉ IP</param>
    /// <param name="userAgent">User agent của trình duyệt</param>
    /// <param name="isSuccess">Đăng nhập thành công hay không</param>
    /// <param name="failureReason">Lý do thất bại nếu đăng nhập không thành công</param>
    /// <param name="cancellationToken">Token hủy</param>
    Task LogLoginAsync(Guid userId, string username, string ipAddress, string userAgent, bool isSuccess, string? failureReason = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ghi log sự kiện hệ thống
    /// </summary>
    /// <param name="eventType">Loại sự kiện (login, error, warn, ok)</param>
    /// <param name="device">Thiết bị liên quan</param>
    /// <param name="account">Tài khoản liên quan (nếu có)</param>
    /// <param name="description">Mô tả sự kiện</param>
    /// <param name="additionalData">Dữ liệu bổ sung (JSON string)</param>
    /// <param name="cancellationToken">Token hủy</param>
    Task LogEventAsync(string eventType, string device, string? account, string description, string? additionalData = null, CancellationToken cancellationToken = default);
}
