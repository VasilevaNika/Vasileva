using WebApplication1.Models.DTO;

namespace WebApplication1.Services.Interfaces;

public interface ITripService
{
    Task<TripDto?> GetByIdAsync(int id, int? userId);
    Task<PagedResponseDto<TripDto>> GetPagedAsync(int page, int pageSize, TripFilterDto? filter, int? userId);
    Task<TripDto> CreateAsync(CreateTripDto dto, int userId);
    Task<TripDto> UpdateAsync(int id, UpdateTripDto dto, int userId, List<string> permissions);
    Task DeleteAsync(int id, int userId, List<string> permissions);
}

