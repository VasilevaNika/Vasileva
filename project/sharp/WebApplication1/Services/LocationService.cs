using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<LocationService> _logger;

    public LocationService(
        ILocationRepository locationRepository,
        ILogger<LocationService> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<LocationDto?> GetByIdAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        return location == null ? null : MapToDto(location);
    }

    public async Task<List<LocationDto>> GetAllAsync()
    {
        var locations = await _locationRepository.GetAllAsync();
        return locations.Select(MapToDto).ToList();
    }

    public async Task<LocationDto> CreateAsync(CreateLocationDto dto, List<string> permissions)
    {
        if (!permissions.Contains("locations.create"))
        {
            throw new UnauthorizedAccessException("You don't have permission to create locations");
        }

        var location = new Location
        {
            Name = dto.Name,
            Country = dto.Country,
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Description = dto.Description
        };

        location = await _locationRepository.CreateAsync(location);

        _logger.LogInformation("Created location {LocationId}", location.Id);

        return MapToDto(location);
    }

    public async Task<LocationDto> UpdateAsync(int id, UpdateLocationDto dto, List<string> permissions)
    {
        if (!permissions.Contains("locations.update"))
        {
            throw new UnauthorizedAccessException("You don't have permission to update locations");
        }

        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
        {
            throw new KeyNotFoundException($"Location with id {id} not found");
        }

        location.Name = dto.Name;
        location.Country = dto.Country;
        location.City = dto.City;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.Description = dto.Description;

        location = await _locationRepository.UpdateAsync(location);

        _logger.LogInformation("Updated location {LocationId}", location.Id);

        return MapToDto(location);
    }

    public async Task DeleteAsync(int id, List<string> permissions)
    {
        if (!permissions.Contains("locations.delete"))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete locations");
        }

        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
        {
            throw new KeyNotFoundException($"Location with id {id} not found");
        }

        await _locationRepository.DeleteAsync(id);

        _logger.LogInformation("Deleted location {LocationId}", id);
    }

    private LocationDto MapToDto(Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Country = location.Country,
            City = location.City,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Description = location.Description,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}

