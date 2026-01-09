namespace WebApplication1.Models.Entities;

public class TripTag
{
    public int TripId { get; set; }
    public int TagId { get; set; }

    // Navigation properties
    public Trip Trip { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}

