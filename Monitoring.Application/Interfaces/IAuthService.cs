
namespace Monitoring.Application.Interfaces;

/// <summary>
/// Interface dịch vụ xử lý xác thực và đăng nhập
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng nhập và tạo JWT token
    /// </summary>
    /// <param name="request">Thông tin đăng nhập</param>
    /// <param name="ipAddress">Địa chỉ IP của client</param>
    /// <param name="userAgent">User agent của client</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin đăng nhập và JWT token, null nếu đăng nhập thất bại</returns>
    Task<LoginResponse?> LoginAsync(LoginRequest request, string ipAddress, string userAgent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy danh sách quyền của người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách quyền của người dùng</returns>
    Task<IList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
