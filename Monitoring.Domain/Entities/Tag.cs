namespace Monitoring.Domain.Entities;

/// <summary>
/// Tag entity representing a monitored industrial tag (512 tags total)
/// </summary>
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string DataType { get; set; } = "Double"; // Double, Int, Bool, String
    public int PumpId { get; set; } // 1, 2, 3, etc.
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<TagHistory> TagHistories { get; set; } = new List<TagHistory>();
}
