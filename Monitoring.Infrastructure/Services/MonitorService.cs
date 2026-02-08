
namespace Monitoring.Infrastructure.Services;

public class MonitorService : IMonitorService
{
    private readonly MonitoringDbContext _context;

    public MonitorService(MonitoringDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MonitorRowDto>> GetMonitorDataAsync(
        string pumpId,
        DateTime fromDate,
        DateTime toDate,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        // Validate pagination parameters
        page = Math.Max(1, page);
        pageSize = Math.Max(1, Math.Min(1000, pageSize)); // Limit max pageSize to 1000

        if (!int.TryParse(pumpId.Replace("pump", ""), out var pumpNumber))
        {
            return new PagedResult<MonitorRowDto>
            {
                Items = Enumerable.Empty<MonitorRowDto>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        // from: 00:00:00, to: exclusive end-of-day (next day at 00:00) để tránh phải nhập lệch ngày
        var from = fromDate.Date;
        var to = toDate.Date.AddDays(1);

        // Get tag histories for the specified pump and date range
        var histories = await _context.TagHistories
            .Include(th => th.Tag)
            .Where(th => th.PumpId == pumpNumber && th.Timestamp >= from && th.Timestamp <= to)
            .OrderByDescending(th => th.Timestamp)
            .ToListAsync(cancellationToken);

        // Group by exact timestamp (1 minute per row)
        var grouped = histories
            .GroupBy(th => new { th.Timestamp.Date, Hour = th.Timestamp.Hour, Minute = th.Timestamp.Minute }) // Group by 1-minute intervals
            .Select(g => new
            {
                Timestamp = new DateTime(g.Key.Date.Year, g.Key.Date.Month, g.Key.Date.Day, g.Key.Hour, g.Key.Minute, 0),
                Tags = g.ToDictionary(t => t.Tag.Name, t => t.GetValue())
            })
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        var allRows = new List<MonitorRowDto>();

        foreach (var group in grouped)
        {
            var tags = group.Tags;
            var timestamp = group.Timestamp;

            allRows.Add(new MonitorRowDto(
                PumpId: pumpId,
                Timestamp: timestamp,
                Date: timestamp.ToString("dd/MM/yyyy"),
                Time: timestamp.ToString("HH:mm:ss"),
                TempA: GetTagValue(tags, "TempA", "0.0"),
                TempB: GetTagValue(tags, "TempB", "0.0"),
                TempC: GetTagValue(tags, "TempC", "0.0"),
                Vrs: GetTagValue(tags, "Vrs", "0"),
                Vst: GetTagValue(tags, "Vst", "0"),
                Vtr: GetTagValue(tags, "Vtr", "0"),
                CurrentR: GetTagValue(tags, "CurrentR", "0.0"),
                CurrentS: GetTagValue(tags, "CurrentS", "0.0"),
                CurrentT: GetTagValue(tags, "CurrentT", "0.0"),
                Runtime: GetTagValue(tags, "Runtime", "00:00"),
                TankOut: GetTagValue(tags, "TankOut", "0"),
                TankIn: GetTagValue(tags, "TankIn", "0")
            ));
        }

        var totalCount = allRows.Count;
        
        // Apply pagination
        var pagedItems = allRows
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<MonitorRowDto>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // Helper method for export (gets all data without pagination)
    private async Task<IEnumerable<MonitorRowDto>> GetAllMonitorDataAsync(
        string pumpId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var result = await GetMonitorDataAsync(pumpId, fromDate, toDate, 1, int.MaxValue, cancellationToken);
        return result.Items;
    }

    public async Task<IEnumerable<PumpOptionDto>> GetPumpsAsync(CancellationToken cancellationToken = default)
    {
        var pumps = await _context.Tags
            .Where(t => t.IsActive)
            .Select(t => t.PumpId)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync(cancellationToken);

        return pumps.Select(p => new PumpOptionDto($"pump{p}", $"Bom {p}"));
    }

    public async Task<byte[]> ExportMonitorDataAsync(string pumpId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        var data = await GetAllMonitorDataAsync(pumpId, fromDate, toDate, cancellationToken);
        
        // Excel export using EPPlus
        using var package = new OfficeOpenXml.ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Giám Sát");

        // Set header row
        worksheet.Cells[1, 1].Value = "Ngày";
        worksheet.Cells[1, 2].Value = "Giờ";
        worksheet.Cells[1, 3].Value = "NĐ Cuộn A (°C)";
        worksheet.Cells[1, 4].Value = "NĐ Cuộn B (°C)";
        worksheet.Cells[1, 5].Value = "NĐ Cuộn C (°C)";
        worksheet.Cells[1, 6].Value = "Đ. Áp RS (V)";
        worksheet.Cells[1, 7].Value = "Đ. Áp ST (V)";
        worksheet.Cells[1, 8].Value = "Đ. Áp TR (V)";
        worksheet.Cells[1, 9].Value = "Đ.Điện R";
        worksheet.Cells[1, 10].Value = "Đ.Điện S";
        worksheet.Cells[1, 11].Value = "Đ.Điện T";
        worksheet.Cells[1, 12].Value = "T.Gian H.Động";
        worksheet.Cells[1, 13].Value = "Mức Bể Xả";
        worksheet.Cells[1, 14].Value = "Mức Bể Hút";

        // Style header
        using (var range = worksheet.Cells[1, 1, 1, 14])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Add data rows
        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cells[row, 1].Value = item.Date;
            worksheet.Cells[row, 2].Value = item.Time;
            worksheet.Cells[row, 3].Value = item.TempA;
            worksheet.Cells[row, 4].Value = item.TempB;
            worksheet.Cells[row, 5].Value = item.TempC;
            worksheet.Cells[row, 6].Value = item.Vrs;
            worksheet.Cells[row, 7].Value = item.Vst;
            worksheet.Cells[row, 8].Value = item.Vtr;
            worksheet.Cells[row, 9].Value = item.CurrentR;
            worksheet.Cells[row, 10].Value = item.CurrentS;
            worksheet.Cells[row, 11].Value = item.CurrentT;
            worksheet.Cells[row, 12].Value = item.Runtime;
            worksheet.Cells[row, 13].Value = item.TankOut;
            worksheet.Cells[row, 14].Value = item.TankIn;
            row++;
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }

    private static string GetTagValue(Dictionary<string, object?> tags, string tagName, string defaultValue)
    {
        if (tags.TryGetValue(tagName, out var value) && value != null)
        {
            return value switch
            {
                double d => d.ToString("0.0"),
                int i => i.ToString("0"),
                _ => value.ToString() ?? defaultValue
            };
        }
        return defaultValue;
    }
}
