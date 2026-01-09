using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Interfaces;

public interface ITravelJournalRepository
{
    Task<TravelJournal?> GetByIdAsync(int id);
    Task<List<TravelJournal>> GetByUserIdAsync(int userId);
    Task<TravelJournal> CreateAsync(TravelJournal journal);
    Task<TravelJournal> UpdateAsync(TravelJournal journal);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

