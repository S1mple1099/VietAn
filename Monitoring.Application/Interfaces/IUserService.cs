namespace Monitoring.Application.Interfaces;

/// <summary>
/// Interface dịch vụ quản lý người dùng và vai trò
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Lấy danh sách tất cả người dùng
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách người dùng</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy</returns>
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tạo người dùng mới
    /// </summary>
    /// <param name="request">Thông tin người dùng mới</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin người dùng đã tạo</returns>
    /// <exception cref="InvalidOperationException">Khi username đã tồn tại</exception>
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin người dùng đã cập nhật</returns>
    /// <exception cref="InvalidOperationException">Khi không tìm thấy người dùng</exception>
    Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Xóa người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <exception cref="InvalidOperationException">Khi không tìm thấy người dùng</exception>
    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Đổi mật khẩu người dùng
    /// </summary>
    /// <param name="userId">ID của người dùng</param>
    /// <param name="request">Thông tin mật khẩu mới và mật khẩu cũ</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <exception cref="InvalidOperationException">Khi không tìm thấy người dùng</exception>
    /// <exception cref="UnauthorizedAccessException">Khi mật khẩu cũ không đúng</exception>
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy danh sách tất cả vai trò
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách vai trò</returns>
    Task<IEnumerable<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy thông tin vai trò theo ID
    /// </summary>
    /// <param name="roleId">ID của vai trò</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin vai trò hoặc null nếu không tìm thấy</returns>
    Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}
