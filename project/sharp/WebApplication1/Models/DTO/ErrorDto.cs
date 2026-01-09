namespace WebApplication1.Models.DTO;

public class ErrorDto
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
}

