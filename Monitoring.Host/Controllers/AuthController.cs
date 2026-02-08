
namespace Monitoring.Host.Controllers;

/// <summary>
/// Controller xử lý xác thực và đăng nhập
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng nhập và nhận JWT token
    /// </summary>
    /// <param name="request">Thông tin đăng nhập (username, password)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>JWT token và thông tin user nếu đăng nhập thành công</returns>
    /// <response code="200">Đăng nhập thành công</response>
    /// <response code="401">Thông tin đăng nhập không hợp lệ</response>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = Request.Headers["User-Agent"].ToString();

        var response = await _authService.LoginAsync(request, ipAddress, userAgent, cancellationToken);

        if (response == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(response);
    }
}
