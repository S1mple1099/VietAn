
namespace Monitoring.Host.Controllers;

/// <summary>
/// Controller quản lý lịch sử sự kiện và đăng nhập
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    /// <summary>
    /// Lấy lịch sử sự kiện và đăng nhập (GET)
    /// </summary>
    /// <param name="request">Thông tin filter (EventType, SearchText, FromDate, ToDate, IncludeLoginLogs, Page, PageSize)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân trang lịch sử sự kiện</returns>
    /// <response code="200">Trả về lịch sử sự kiện thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ViewHistory</response>
    [HttpGet]
    [Authorize(Policy = "ViewHistory")]
    public async Task<ActionResult> GetHistory(
        GetHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;
        var eventTypeFilter = string.IsNullOrWhiteSpace(request.EventType) || request.EventType == "all" ? null : request.EventType;

        var data = await _historyService.GetHistoryAsync(
            eventTypeFilter, 
            request.SearchText, 
            from, 
            to, 
            request.IncludeLoginLogs,
            request.Page,
            request.PageSize,
            cancellationToken);
        return Ok(data);
    }

    /// <summary>
    /// Tìm kiếm lịch sử sự kiện và đăng nhập (POST)
    /// </summary>
    /// <param name="request">Thông tin filter (EventType, SearchText, FromDate, ToDate, IncludeLoginLogs, Page, PageSize)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả phân trang lịch sử sự kiện</returns>
    /// <response code="200">Trả về lịch sử sự kiện thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ViewHistory</response>
    [HttpPost("search")]
    //[Authorize(Policy = "ViewHistory")]
    public async Task<ActionResult> SearchHistory(
        [FromBody] GetHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;
        var eventTypeFilter = string.IsNullOrWhiteSpace(request.EventType) || request.EventType == "all" ? null : request.EventType;

        var data = await _historyService.GetHistoryAsync(
            eventTypeFilter, 
            request.SearchText, 
            from, 
            to, 
            request.IncludeLoginLogs,
            request.Page,
            request.PageSize,
            cancellationToken);
        return Ok(data);
    }

    /// <summary>
    /// Lấy danh sách các loại sự kiện
    /// </summary>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Danh sách loại sự kiện</returns>
    /// <response code="200">Trả về danh sách loại sự kiện thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ViewHistory</response>
    [HttpGet("event-types")]
    [Authorize(Policy = "ViewHistory")]
    public async Task<ActionResult> GetEventTypes(CancellationToken cancellationToken)
    {
        var eventTypes = await _historyService.GetEventTypesAsync(cancellationToken);
        return Ok(eventTypes);
    }

    /// <summary>
    /// Xuất lịch sử sự kiện ra file Excel (GET)
    /// </summary>
    /// <param name="request">Thông tin filter (EventType, SearchText, FromDate, ToDate, IncludeLoginLogs)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>File Excel chứa lịch sử sự kiện</returns>
    /// <response code="200">Xuất file Excel thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ExportData</response>
    [HttpGet("export")]
    [Authorize(Policy = "ExportData")]
    public async Task<ActionResult> ExportHistory(
        GetHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;
        var eventTypeFilter = string.IsNullOrWhiteSpace(request.EventType) || request.EventType == "all" ? null : request.EventType;

        var data = await _historyService.ExportHistoryAsync(eventTypeFilter, request.SearchText, from, to, request.IncludeLoginLogs, cancellationToken);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"history_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    /// <summary>
    /// Xuất lịch sử sự kiện ra file Excel (POST)
    /// </summary>
    /// <param name="request">Thông tin filter (EventType, SearchText, FromDate, ToDate, IncludeLoginLogs)</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>File Excel chứa lịch sử sự kiện</returns>
    /// <response code="200">Xuất file Excel thành công</response>
    /// <response code="401">Chưa đăng nhập</response>
    /// <response code="403">Không có quyền ExportData</response>
    [HttpPost("export")]
    [Authorize(Policy = "ExportData")]
    public async Task<ActionResult> ExportHistoryPost(
        [FromBody] GetHistoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var from = request.FromDate ?? DateTime.Today;
        var to = request.ToDate ?? DateTime.Today;
        var eventTypeFilter = string.IsNullOrWhiteSpace(request.EventType) || request.EventType == "all" ? null : request.EventType;

        var data = await _historyService.ExportHistoryAsync(eventTypeFilter, request.SearchText, from, to, request.IncludeLoginLogs, cancellationToken);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"history_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }
}
