using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(int id);
    Task<List<Location>> GetAllAsync();
    Task<Location> CreateAsync(Location location);
    Task<Location> UpdateAsync(Location location);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

