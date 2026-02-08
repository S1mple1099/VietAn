namespace Monitoring.Domain.Entities;

/// <summary>
/// Tag history entity for storing historical tag values (partitioned by month)
/// </summary>
public class TagHistory
{
    public long Id { get; set; }
    public int TagId { get; set; }
    public int PumpId { get; set; }
    public DateTime Timestamp { get; set; }
    public double? ValueDouble { get; set; }
    public int? ValueInt { get; set; }
    public bool? ValueBool { get; set; }
    public string? ValueString { get; set; }
    public string Quality { get; set; } = "Good"; // Good, Bad, Uncertain

    // Navigation property
    public Tag Tag { get; set; } = null!;

    /// <summary>
    /// Gets the value as object based on tag data type
    /// </summary>
    public object? GetValue()
    {
        return ValueDouble ?? (object?)ValueInt ?? (object?)ValueBool ?? ValueString;
    }
}
