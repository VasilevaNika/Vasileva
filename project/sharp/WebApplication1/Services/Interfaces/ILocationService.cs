using WebApplication1.Models.DTO;

namespace WebApplication1.Services.Interfaces;

public interface ILocationService
{
    Task<LocationDto?> GetByIdAsync(int id);
    Task<List<LocationDto>> GetAllAsync();
    Task<LocationDto> CreateAsync(CreateLocationDto dto, List<string> permissions);
    Task<LocationDto> UpdateAsync(int id, UpdateLocationDto dto, List<string> permissions);
    Task DeleteAsync(int id, List<string> permissions);
}

