namespace WebApplication1.Models.Entities;

public class Trip
{
    public int Id { get; set; }
    public int TravelJournalId { get; set; }
    public int LocationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Rating { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public TravelJournal TravelJournal { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public ICollection<TripTag> TripTags { get; set; } = new List<TripTag>();
}

