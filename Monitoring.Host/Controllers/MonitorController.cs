
namespace Monitoring.Host.Controllers;

/// <summary>
/// Controller quản lý dữ liệu giám sát các máy bơm
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonitorController : ControllerBase
{
    private readonly IMonitorService _monitorService;

    public MonitorController(IMonitorService monitorService)
    {
        _monitorService = monitorService;
    }

    /// <summary>
    /// Lấy dữ liệu giám sát theo máy bơm và khoảng thời gian (GET)
    /// </summary>
    /// <param name="request">Thông tin filter (PumpId, FromDate, ToDate, Page, PageSize)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân trang dữ liệu giám sát</returns>
    /// <response code="200">Trả về dữ liệu giám sát thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ViewMonitor</response>
    [HttpGet("data")]
    [Authorize(Policy = "ViewMonitor")]
    public async Task<ActionResult> GetMonitorData(
        GetMonitorDataRequest request,
        CancellationToken cancellationToken = default)
    {
        var pump = request.PumpId ?? "pump1";
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;

        var data = await _monitorService.GetMonitorDataAsync(pump, from, to, request.Page, request.PageSize, cancellationToken);
        return Ok(data);
    }

    /// <summary>
    /// Lấy dữ liệu giám sát theo máy bơm và khoảng thời gian (POST)
    /// </summary>
    /// <param name="request">Thông tin filter (PumpId, FromDate, ToDate, Page, PageSize)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân trang dữ liệu giám sát</returns>
    /// <response code="200">Trả về dữ liệu giám sát thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ViewMonitor</response>
    [HttpPost("data")]
    [Authorize(Policy = "ViewMonitor")]
    public async Task<ActionResult> GetMonitorDataPost(
        [FromBody] GetMonitorDataRequest request,
        CancellationToken cancellationToken = default)
    {
        var pump = request.PumpId ?? "pump1";
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;

        var data = await _monitorService.GetMonitorDataAsync(pump, from, to, request.Page, request.PageSize, cancellationToken);
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách tất cả các máy bơm
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách máy bơm</returns>
    /// <response code="200">Trả về danh sách máy bơm thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ViewMonitor</response>
    [HttpGet("pumps")]
    [Authorize(Policy = "ViewMonitor")]
    public async Task<ActionResult> GetPumps(CancellationToken cancellationToken)
    {
        var pumps = await _monitorService.GetPumpsAsync(cancellationToken);
        return Ok(pumps);
    }

    /// <summary>
    /// Xuất dữ liệu giám sát ra file Excel (GET)
    /// </summary>
    /// <param name="request">Thông tin filter (PumpId, FromDate, ToDate)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>File Excel chứa dữ liệu giám sát</returns>
    /// <response code="200">Xuất file Excel thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ExportData</response>
    [HttpGet("export")]
    [Authorize(Policy = "ExportData")]
    public async Task<ActionResult> ExportData(
        GetMonitorDataRequest request,
        CancellationToken cancellationToken = default)
    {
        var pump = request.PumpId ?? "pump1";
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;

        var data = await _monitorService.ExportMonitorDataAsync(pump, from, to, cancellationToken);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"monitor_{pump}_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
    }

    /// <summary>
    /// Xuất dữ liệu giám sát ra file Excel (POST)
    /// </summary>
    /// <param name="request">Thông tin filter (PumpId, FromDate, ToDate)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>File Excel chứa dữ liệu giám sát</returns>
    /// <response code="200">Xuất file Excel thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ExportData</response>
    [HttpPost("export")]
    [Authorize(Policy = "ExportData")]
    public async Task<ActionResult> ExportDataPost(
        [FromBody] GetMonitorDataRequest request,
        CancellationToken cancellationToken = default)
    {
        var pump = request.PumpId ?? "pump1";
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;

        var data = await _monitorService.ExportMonitorDataAsync(pump, from, to, cancellationToken);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"monitor_{pump}_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
    }
}
