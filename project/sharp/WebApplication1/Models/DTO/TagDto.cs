namespace WebApplication1.Models.DTO;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#3498db";
    public DateTime CreatedAt { get; set; }
}

