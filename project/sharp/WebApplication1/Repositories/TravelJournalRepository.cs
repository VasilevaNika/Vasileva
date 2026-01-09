using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class TravelJournalRepository : ITravelJournalRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TravelJournalRepository> _logger;

    public TravelJournalRepository(ApplicationDbContext context, ILogger<TravelJournalRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TravelJournal?> GetByIdAsync(int id)
    {
        return await _context.TravelJournals
            .Include(j => j.User)
            .Include(j => j.Trips)
                .ThenInclude(t => t.Location)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<List<TravelJournal>> GetByUserIdAsync(int userId)
    {
        return await _context.TravelJournals
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<TravelJournal> CreateAsync(TravelJournal journal)
    {
        journal.CreatedAt = DateTime.UtcNow;
        journal.UpdatedAt = DateTime.UtcNow;
        _context.TravelJournals.Add(journal);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created travel journal with id {JournalId}", journal.Id);
        return journal;
    }

    public async Task<TravelJournal> UpdateAsync(TravelJournal journal)
    {
        // Ensure all DateTime properties are UTC
        if (journal.CreatedAt.Kind != DateTimeKind.Utc)
        {
            journal.CreatedAt = journal.CreatedAt.Kind == DateTimeKind.Local 
                ? journal.CreatedAt.ToUniversalTime() 
                : DateTime.SpecifyKind(journal.CreatedAt, DateTimeKind.Utc);
        }
        journal.UpdatedAt = DateTime.UtcNow;
        
        _context.TravelJournals.Update(journal);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated travel journal with id {JournalId}", journal.Id);
        return journal;
    }

    public async Task DeleteAsync(int id)
    {
        var journal = await _context.TravelJournals.FindAsync(id);
        if (journal != null)
        {
            _context.TravelJournals.Remove(journal);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted travel journal with id {JournalId}", id);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TravelJournals.AnyAsync(j => j.Id == id);
    }
}

