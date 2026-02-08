namespace Monitoring.Host.Controllers;

/// <summary>
/// Controller quản lý người dùng và vai trò
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ManageUser")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Lấy danh sách tất cả người dùng
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách người dùng</returns>
    /// <response code="200">Trả về danh sách người dùng thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Lấy thông tin người dùng theo ID
    /// </summary>
    /// <param name="id">ID của người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin người dùng</returns>
    /// <response code="200">Trả về thông tin người dùng thành công</response>
    /// <response code="404">Không tìm thấy người dùng</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Tạo người dùng mới
    /// </summary>
    /// <param name="request">Thông tin người dùng mới</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin người dùng đã tạo</returns>
    /// <response code="201">Tạo người dùng thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    /// <param name="id">ID của người dùng</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin người dùng đã cập nhật</returns>
    /// <response code="200">Cập nhật thành công</response>
    /// <response code="404">Không tìm thấy người dùng</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request, cancellationToken);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa người dùng
    /// </summary>
    /// <param name="id">ID của người dùng</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Không có nội dung</returns>
    /// <response code="204">Xóa thành công</response>
    /// <response code="404">Không tìm thấy người dùng</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeleteUserAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Đổi mật khẩu người dùng
    /// </summary>
    /// <param name="id">ID của người dùng</param>
    /// <param name="request">Thông tin mật khẩu mới và mật khẩu cũ</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả đổi mật khẩu</returns>
    /// <response code="200">Đổi mật khẩu thành công</response>
    /// <response code="401">Mật khẩu cũ không đúng hoặc chưa đăng nhập</response>
    /// <response code="404">Không tìm thấy người dùng</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.ChangePasswordAsync(id, request, cancellationToken);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách tất cả vai trò
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách vai trò</returns>
    /// <response code="200">Trả về danh sách vai trò thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken cancellationToken)
    {
        var roles = await _userService.GetAllRolesAsync(cancellationToken);
        return Ok(roles);
    }

    /// <summary>
    /// Lấy thông tin vai trò theo ID
    /// </summary>
    /// <param name="id">ID của vai trò</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Thông tin vai trò</returns>
    /// <response code="200">Trả về thông tin vai trò thành công</response>
    /// <response code="404">Không tìm thấy vai trò</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ManageUser</response>
    [HttpGet("roles/{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id, CancellationToken cancellationToken)
    {
        var role = await _userService.GetRoleByIdAsync(id, cancellationToken);
        if (role == null)
            return NotFound();

        return Ok(role);
    }
}
