using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        ILocationService locationService,
        ILogger<LocationsController> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    private List<string> GetPermissions()
    {
        var permissionsClaim = User.FindFirst("permissions")?.Value;
        return permissionsClaim?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<LocationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LocationDto>>> GetLocations()
    {
        var locations = await _locationService.GetAllAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationDto>> GetLocation(int id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationDto dto)
    {
        var permissions = GetPermissions();
        var location = await _locationService.CreateAsync(dto, permissions);
        return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LocationDto>> UpdateLocation(int id, [FromBody] UpdateLocationDto dto)
    {
        var permissions = GetPermissions();
        var location = await _locationService.UpdateAsync(id, dto, permissions);
        return Ok(location);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var permissions = GetPermissions();
        await _locationService.DeleteAsync(id, permissions);
        return NoContent();
    }
}

