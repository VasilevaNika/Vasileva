using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Interfaces;

public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(int id);
    Task<PagedResponseDto<Trip>> GetPagedAsync(int page, int pageSize, TripFilterDto? filter = null);
    Task<Trip> CreateAsync(Trip trip);
    Task<Trip> UpdateAsync(Trip trip);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

