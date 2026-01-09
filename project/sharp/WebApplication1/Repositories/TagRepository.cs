using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TagRepository> _logger;

    public TagRepository(ApplicationDbContext context, ILogger<TagRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Tags
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        tag.CreatedAt = DateTime.UtcNow;
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created tag with id {TagId}", tag.Id);
        return tag;
    }
}

