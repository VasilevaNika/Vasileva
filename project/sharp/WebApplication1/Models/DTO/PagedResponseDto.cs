namespace WebApplication1.Models.DTO;

public class PagedResponseDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class TripFilterDto
{
    public string? Search { get; set; }
    public int? LocationId { get; set; }
    public int? TravelJournalId { get; set; }
    public int? MinRating { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public List<int>? TagIds { get; set; }
}

