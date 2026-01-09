using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class TravelJournalService : ITravelJournalService
{
    private readonly ITravelJournalRepository _journalRepository;
    private readonly ILogger<TravelJournalService> _logger;

    public TravelJournalService(
        ITravelJournalRepository journalRepository,
        ILogger<TravelJournalService> logger)
    {
        _journalRepository = journalRepository;
        _logger = logger;
    }

    public async Task<TravelJournalDto?> GetByIdAsync(int id, int? userId)
    {
        var journal = await _journalRepository.GetByIdAsync(id);
        if (journal == null)
        {
            return null;
        }

        // Check access
        if (userId.HasValue && journal.UserId != userId.Value)
        {
            // This will be checked by authorization in controller
        }

        return MapToDto(journal);
    }

    public async Task<List<TravelJournalDto>> GetByUserIdAsync(int userId)
    {
        var journals = await _journalRepository.GetByUserIdAsync(userId);
        return journals.Select(MapToDto).ToList();
    }

    public async Task<TravelJournalDto> CreateAsync(CreateTravelJournalDto dto, int userId)
    {
        var journal = new TravelJournal
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description
        };

        journal = await _journalRepository.CreateAsync(journal);

        _logger.LogInformation("User {UserId} created travel journal {JournalId}", userId, journal.Id);

        return MapToDto(journal);
    }

    public async Task<TravelJournalDto> UpdateAsync(int id, UpdateTravelJournalDto dto, int userId, List<string> permissions)
    {
        var journal = await _journalRepository.GetByIdAsync(id);
        if (journal == null)
        {
            throw new KeyNotFoundException($"Travel journal with id {id} not found");
        }

        // Check permissions
        if (journal.UserId != userId && !permissions.Contains("journals.update"))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this journal");
        }

        journal.Title = dto.Title;
        journal.Description = dto.Description;

        journal = await _journalRepository.UpdateAsync(journal);

        _logger.LogInformation("User {UserId} updated travel journal {JournalId}", userId, journal.Id);

        return MapToDto(journal);
    }

    public async Task DeleteAsync(int id, int userId, List<string> permissions)
    {
        var journal = await _journalRepository.GetByIdAsync(id);
        if (journal == null)
        {
            throw new KeyNotFoundException($"Travel journal with id {id} not found");
        }

        // Check permissions
        if (journal.UserId != userId && !permissions.Contains("journals.delete"))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this journal");
        }

        await _journalRepository.DeleteAsync(id);

        _logger.LogInformation("User {UserId} deleted travel journal {JournalId}", userId, id);
    }

    private TravelJournalDto MapToDto(TravelJournal journal)
    {
        return new TravelJournalDto
        {
            Id = journal.Id,
            UserId = journal.UserId,
            Title = journal.Title,
            Description = journal.Description,
            CreatedAt = journal.CreatedAt,
            UpdatedAt = journal.UpdatedAt
        };
    }
}

