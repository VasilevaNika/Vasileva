using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocationRepository> _logger;

    public LocationRepository(ApplicationDbContext context, ILogger<LocationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await _context.Locations.FindAsync(id);
    }

    public async Task<List<Location>> GetAllAsync()
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Location> CreateAsync(Location location)
    {
        location.CreatedAt = DateTime.UtcNow;
        location.UpdatedAt = DateTime.UtcNow;
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created location with id {LocationId}", location.Id);
        return location;
    }

    public async Task<Location> UpdateAsync(Location location)
    {
        location.UpdatedAt = DateTime.UtcNow;
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated location with id {LocationId}", location.Id);
        return location;
    }

    public async Task DeleteAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location != null)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted location with id {LocationId}", id);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Locations.AnyAsync(l => l.Id == id);
    }
}

