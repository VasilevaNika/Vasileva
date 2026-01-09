namespace WebApplication1.Models.DTO;

public class TravelJournalDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTravelJournalDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateTravelJournalDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

