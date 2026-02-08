
using System.Globalization;

namespace Monitoring.Infrastructure.Services;

public class HistoryService : IHistoryService
{
    private readonly MonitoringDbContext _context;

    public HistoryService(MonitoringDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<HistoryRowDto>> GetHistoryAsync(
        string? eventType,
        string? searchText,
        DateTime? fromDate,
        DateTime? toDate,
        bool includeLoginLogs = false,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        page = Math.Max(1, page);
        pageSize = Math.Max(1, Math.Min(1000, pageSize)); // Limit max pageSize to 1000

        var query = _context.EventLogs.AsQueryable();

        // Filter by event type (EventType trong DB là: login, error, warn, ok)
        if (!string.IsNullOrWhiteSpace(eventType) && eventType != "all")
        {
            query = query.Where(e => e.EventType == eventType);
        }

        // Filter by date range (default: today if not specified)
        // from: 00:00:00, to: exclusive end-of-day (next day at 00:00) để tránh phải nhập lệch ngày
        var from = (fromDate ?? DateTime.Today).Date;
        var to = (toDate ?? DateTime.Today).Date.AddDays(1);
        
        query = query.Where(e => e.Timestamp >= from && e.Timestamp < to);

        // Search text
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var search = searchText.Trim();
            query = query.Where(e =>
                e.Id.ToString().Contains(search) ||
                e.Timestamp.ToString().Contains(search) ||
                e.Device.Contains(search) ||
                (e.Account != null && e.Account.Contains(search)) ||
                e.EventType.Contains(search) ||
                e.Description.Contains(search));
        }

        var events = await query
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync(cancellationToken);

        // Build list with real timestamp so we can sort newest -> oldest correctly,
        // then assign display Id after sorting.
        var itemsWithTimestamp = new List<(DateTime Timestamp, HistoryRowDto Row)>();

        // Add event logs - Map EventType từ DB sang hiển thị
        var eventTypeDisplayMap = new Dictionary<string, string>
        {
            { "login", "Đăng Nhập" },
            { "error", "Lỗi" },
            { "ok", "Hoạt Động Tốt" },
            { "warn", "Cảnh Báo" }
        };

        itemsWithTimestamp.AddRange(events.Select(e =>
        (
            e.Timestamp,
            new HistoryRowDto(
                Id: "", // will be assigned after sorting
                Time: e.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
                Device: e.Device,
                Account: e.Account ?? "",
                Type: eventTypeDisplayMap.TryGetValue(e.EventType, out var displayType) ? displayType : e.EventType,
                Description: e.Description,
                ErrorCode: e.ErrorCode,
                ProcessingTime: e.ProcessingTimeSeconds.HasValue ? $"{e.ProcessingTimeSeconds.Value}s" : null)
        )));

        // Add login logs if requested
        if (includeLoginLogs || eventType == "login" || eventType == "all")
        {
            var loginQuery = _context.LoginLogs.AsQueryable();

            // Apply same date filters (exclusive end-of-day)
            var loginFrom = (fromDate ?? DateTime.Today).Date;
            var loginTo = (toDate ?? DateTime.Today).Date.AddDays(1);
            
            loginQuery = loginQuery.Where(l => l.Timestamp >= loginFrom && l.Timestamp < loginTo);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var search = searchText.Trim();
                loginQuery = loginQuery.Where(l =>
                    l.Id.ToString().Contains(search) ||
                    l.Timestamp.ToString().Contains(search) ||
                    l.Username.Contains(search) ||
                    l.IpAddress.Contains(search) ||
                    (l.FailureReason != null && l.FailureReason.Contains(search)));
            }

            var loginLogs = await loginQuery
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync(cancellationToken);

            itemsWithTimestamp.AddRange(loginLogs.Select(l =>
            (
                l.Timestamp,
                new HistoryRowDto(
                    Id: "", // will be assigned after sorting
                    Time: l.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"),
                    Device: "Web",
                    Account: l.Username,
                    Type: l.IsSuccess ? "Đăng Nhập" : "Lỗi Đăng Nhập",
                    Description: l.IsSuccess
                        ? $"Đăng nhập thành công từ {l.IpAddress}"
                        : $"Đăng nhập thất bại: {l.FailureReason ?? "Không xác định"}",
                    ErrorCode: l.IsSuccess ? null : "LOGIN_FAILED",
                    ProcessingTime: "0s" // Login thường xử lý ngay
                )
            )));
        }

        // Sort newest -> oldest by actual timestamp
        var allResults = itemsWithTimestamp
            .OrderByDescending(x => x.Timestamp)
            .Select(x => x.Row)
            .Select((row, idx) => row with { Id = (idx + 1).ToString("00") })
            .ToList();
        var totalCount = allResults.Count;
        
        // Apply pagination
        var pagedItems = allResults
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<HistoryRowDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // Helper method for export (gets all data without pagination)
    private async Task<IEnumerable<HistoryRowDto>> GetAllHistoryAsync(
        string? eventType,
        string? searchText,
        DateTime? fromDate,
        DateTime? toDate,
        bool includeLoginLogs = false,
        CancellationToken cancellationToken = default)
    {
        var result = await GetHistoryAsync(eventType, searchText, fromDate, toDate, includeLoginLogs, 1, int.MaxValue, cancellationToken);
        return result.Items;
    }

    public async Task<IEnumerable<EventTypeOptionDto>> GetEventTypesAsync(CancellationToken cancellationToken = default)
    {
        return new[]
        {
            new EventTypeOptionDto("all", "Loại sự kiện"),
            new EventTypeOptionDto("login", "Đăng Nhập"),
            new EventTypeOptionDto("error", "Lỗi"),
            new EventTypeOptionDto("ok", "Hoạt Động Tốt"),
            new EventTypeOptionDto("warn", "Cảnh Báo")
        };
    }

    public async Task<byte[]> ExportHistoryAsync(string? eventType, string? searchText, DateTime? fromDate, DateTime? toDate, bool includeLoginLogs = true, CancellationToken cancellationToken = default)
    {
        var data = await GetAllHistoryAsync(eventType, searchText, fromDate, toDate, includeLoginLogs, cancellationToken);

        // Excel export using EPPlus
        using var package = new OfficeOpenXml.ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Lịch Sử");

        // Set header row
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Thời Gian";
        worksheet.Cells[1, 3].Value = "Thiết Bị";
        worksheet.Cells[1, 4].Value = "Tài Khoản";
        worksheet.Cells[1, 5].Value = "Loại Sự Kiện";
        worksheet.Cells[1, 6].Value = "Mã Lỗi";
        worksheet.Cells[1, 7].Value = "Mô Tả";
        worksheet.Cells[1, 8].Value = "Thời Gian Xử Lý";

        // Style header
        using (var range = worksheet.Cells[1, 1, 1, 8])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Add data rows
        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cells[row, 1].Value = item.Id;
            worksheet.Cells[row, 2].Value = item.Time;
            worksheet.Cells[row, 3].Value = item.Device;
            worksheet.Cells[row, 4].Value = item.Account;
            worksheet.Cells[row, 5].Value = item.Type;
            worksheet.Cells[row, 6].Value = item.ErrorCode ?? "";
            worksheet.Cells[row, 7].Value = item.Description;
            worksheet.Cells[row, 8].Value = item.ProcessingTime ?? "";
            row++;
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }
}
