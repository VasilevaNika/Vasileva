using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;
    private readonly ITravelJournalRepository _journalRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<TripService> _logger;

    public TripService(
        ITripRepository tripRepository,
        ITravelJournalRepository journalRepository,
        ILocationRepository locationRepository,
        ITagRepository tagRepository,
        ILogger<TripService> logger)
    {
        _tripRepository = tripRepository;
        _journalRepository = journalRepository;
        _locationRepository = locationRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<TripDto?> GetByIdAsync(int id, int? userId)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            return null;
        }

        if (userId.HasValue && trip.TravelJournal.UserId != userId.Value)
        {
        }

        return MapToDto(trip);
    }

    public async Task<PagedResponseDto<TripDto>> GetPagedAsync(int page, int pageSize, TripFilterDto? filter, int? userId)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _tripRepository.GetPagedAsync(page, pageSize, filter);
        
        return new PagedResponseDto<TripDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            Total = result.Total,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<TripDto> CreateAsync(CreateTripDto dto, int userId)
    {
        var journal = await _journalRepository.GetByIdAsync(dto.TravelJournalId);
        if (journal == null)
        {
            throw new KeyNotFoundException($"Travel journal with id {dto.TravelJournalId} not found");
        }

        if (journal.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only create trips in your own journals");
        }

        var location = await _locationRepository.GetByIdAsync(dto.LocationId);
        if (location == null)
        {
            throw new KeyNotFoundException($"Location with id {dto.LocationId} not found");
        }

        if (dto.TagIds.Any())
        {
            var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
            if (tags.Count != dto.TagIds.Count)
            {
                throw new KeyNotFoundException("One or more tags not found");
            }
        }

        var startDate = dto.StartDate.Kind == DateTimeKind.Utc 
            ? dto.StartDate 
            : DateTime.SpecifyKind(dto.StartDate.Date, DateTimeKind.Utc);
        var endDate = dto.EndDate.HasValue
            ? (dto.EndDate.Value.Kind == DateTimeKind.Utc 
                ? dto.EndDate 
                : DateTime.SpecifyKind(dto.EndDate.Value.Date, DateTimeKind.Utc))
            : null;

        var trip = new Trip
        {
            TravelJournalId = dto.TravelJournalId,
            LocationId = dto.LocationId,
            Title = dto.Title,
            Description = dto.Description,
            StartDate = startDate,
            EndDate = endDate,
            Rating = dto.Rating,
            PhotoUrls = dto.PhotoUrls
        };

        trip = await _tripRepository.CreateAsync(trip);

        if (dto.TagIds.Any())
        {
            trip.TripTags = dto.TagIds.Select(tagId => new TripTag
            {
                TripId = trip.Id,
                TagId = tagId
            }).ToList();
            trip = await _tripRepository.UpdateAsync(trip);
        }

        trip = await _tripRepository.GetByIdAsync(trip.Id);
        if (trip == null)
        {
            throw new InvalidOperationException("Failed to reload trip after creation");
        }

        _logger.LogInformation("User {UserId} created trip {TripId}", userId, trip.Id);

        return MapToDto(trip);
    }

    public async Task<TripDto> UpdateAsync(int id, UpdateTripDto dto, int userId, List<string> permissions)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException($"Trip with id {id} not found");
        }

        if (trip.TravelJournal.UserId != userId && !permissions.Contains("trips.update"))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this trip");
        }

        // Verify location exists
        var location = await _locationRepository.GetByIdAsync(dto.LocationId);
        if (location == null)
        {
            throw new KeyNotFoundException($"Location with id {dto.LocationId} not found");
        }

        // Verify tags exist
        if (dto.TagIds.Any())
        {
            var tags = await _tagRepository.GetByIdsAsync(dto.TagIds);
            if (tags.Count != dto.TagIds.Count)
            {
                throw new KeyNotFoundException("One or more tags not found");
            }
        }

        var startDate = dto.StartDate.Kind == DateTimeKind.Utc 
            ? dto.StartDate 
            : DateTime.SpecifyKind(dto.StartDate.Date, DateTimeKind.Utc);
        var endDate = dto.EndDate.HasValue
            ? (dto.EndDate.Value.Kind == DateTimeKind.Utc 
                ? dto.EndDate 
                : DateTime.SpecifyKind(dto.EndDate.Value.Date, DateTimeKind.Utc))
            : null;

        trip.LocationId = dto.LocationId;
        trip.Title = dto.Title;
        trip.Description = dto.Description;
        trip.StartDate = startDate;
        trip.EndDate = endDate;
        trip.Rating = dto.Rating;
        trip.PhotoUrls = dto.PhotoUrls;

        // Update tags
        trip.TripTags = dto.TagIds.Select(tagId => new TripTag
        {
            TripId = trip.Id,
            TagId = tagId
        }).ToList();

        trip = await _tripRepository.UpdateAsync(trip);

        trip = await _tripRepository.GetByIdAsync(trip.Id);
        if (trip == null)
        {
            throw new InvalidOperationException("Failed to reload trip after update");
        }

        _logger.LogInformation("User {UserId} updated trip {TripId}", userId, trip.Id);

        return MapToDto(trip);
    }

    public async Task DeleteAsync(int id, int userId, List<string> permissions)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            throw new KeyNotFoundException($"Trip with id {id} not found");
        }

        // Check permissions
        if (trip.TravelJournal.UserId != userId && !permissions.Contains("trips.delete"))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this trip");
        }

        await _tripRepository.DeleteAsync(id);

        _logger.LogInformation("User {UserId} deleted trip {TripId}", userId, id);
    }

    private TripDto MapToDto(Trip trip)
    {
        if (trip == null)
        {
            throw new ArgumentNullException(nameof(trip));
        }

        if (trip.Location == null)
        {
            throw new InvalidOperationException($"Location is not loaded for trip {trip.Id}");
        }

        return new TripDto
        {
            Id = trip.Id,
            TravelJournalId = trip.TravelJournalId,
            LocationId = trip.LocationId,
            Title = trip.Title,
            Description = trip.Description,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            Rating = trip.Rating,
            PhotoUrls = trip.PhotoUrls,
            Location = new LocationDto
            {
                Id = trip.Location.Id,
                Name = trip.Location.Name,
                Country = trip.Location.Country,
                City = trip.Location.City,
                Latitude = trip.Location.Latitude,
                Longitude = trip.Location.Longitude,
                Description = trip.Location.Description,
                CreatedAt = trip.Location.CreatedAt,
                UpdatedAt = trip.Location.UpdatedAt
            },
            Tags = trip.TripTags?.Select(tt => 
            {
                if (tt.Tag == null)
                {
                    throw new InvalidOperationException($"Tag is not loaded for trip tag {tt.TripId}-{tt.TagId}");
                }
                return new TagDto
                {
                    Id = tt.Tag.Id,
                    Name = tt.Tag.Name,
                    Color = tt.Tag.Color,
                    CreatedAt = tt.Tag.CreatedAt
                };
            }).ToList(),
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt
        };
    }
}

