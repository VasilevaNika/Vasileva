using WebApplication1.Models.DTO;

namespace WebApplication1.Services.Interfaces;

public interface ITravelJournalService
{
    Task<TravelJournalDto?> GetByIdAsync(int id, int? userId);
    Task<List<TravelJournalDto>> GetByUserIdAsync(int userId);
    Task<TravelJournalDto> CreateAsync(CreateTravelJournalDto dto, int userId);
    Task<TravelJournalDto> UpdateAsync(int id, UpdateTravelJournalDto dto, int userId, List<string> permissions);
    Task DeleteAsync(int id, int userId, List<string> permissions);
}

