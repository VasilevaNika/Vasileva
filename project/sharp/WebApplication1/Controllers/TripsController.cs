using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebApplication1.Models.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TripsController> _logger;

    public TripsController(
        ITripService tripService,
        IDistributedCache cache,
        ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _cache = cache;
        _logger = logger;
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? int.Parse(userIdClaim) : null;
    }

    private List<string> GetPermissions()
    {
        var permissionsClaim = User.FindFirst("permissions")?.Value;
        return permissionsClaim?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<TripDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<TripDto>>> GetTrips(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? locationId = null,
        [FromQuery] int? travelJournalId = null,
        [FromQuery] int? minRating = null,
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null,
        [FromQuery] List<int>? tagIds = null)
    {
        var userId = GetUserId();
        var cacheKey = $"trips:{page}:{pageSize}:{search}:{locationId}:{travelJournalId}:{minRating}:{startDateFrom}:{startDateTo}:{string.Join(",", tagIds ?? new List<int>())}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return Ok(JsonSerializer.Deserialize<PagedResponseDto<TripDto>>(cached));
        }

        var filter = new TripFilterDto
        {
            Search = search,
            LocationId = locationId,
            TravelJournalId = travelJournalId,
            MinRating = minRating,
            StartDateFrom = startDateFrom,
            StartDateTo = startDateTo,
            TagIds = tagIds
        };

        var result = await _tripService.GetPagedAsync(page, pageSize, filter, userId);

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TripDto>> GetTrip(int id)
    {
        var userId = GetUserId();
        var cacheKey = $"trip:{id}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return Ok(JsonSerializer.Deserialize<TripDto>(cached));
        }

        var trip = await _tripService.GetByIdAsync(id, userId);
        if (trip == null)
        {
            return NotFound();
        }

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(trip), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Ok(trip);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TripDto>> CreateTrip([FromBody] CreateTripDto dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var trip = await _tripService.CreateAsync(dto, userId.Value);

        return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, trip);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TripDto>> UpdateTrip(int id, [FromBody] UpdateTripDto dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var permissions = GetPermissions();
        var trip = await _tripService.UpdateAsync(id, dto, userId.Value, permissions);

        await _cache.RemoveAsync($"trip:{id}");

        return Ok(trip);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var permissions = GetPermissions();
        await _tripService.DeleteAsync(id, userId.Value, permissions);

        await _cache.RemoveAsync($"trip:{id}");

        return NoContent();
    }
}

