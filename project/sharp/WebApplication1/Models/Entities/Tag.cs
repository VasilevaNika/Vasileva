namespace WebApplication1.Models.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#3498db";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<TripTag> TripTags { get; set; } = new List<TripTag>();
}

