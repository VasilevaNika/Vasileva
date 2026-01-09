namespace WebApplication1.Models.DTO;

public class TripDto
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
    public LocationDto Location { get; set; } = null!;
    public List<TagDto> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTripDto
{
    public int TravelJournalId { get; set; }
    public int LocationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Rating { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public List<int> TagIds { get; set; } = new();
}

public class UpdateTripDto
{
    public int LocationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Rating { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public List<int> TagIds { get; set; } = new();
}

