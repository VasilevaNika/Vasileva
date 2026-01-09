using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTO;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TravelJournalsController : ControllerBase
{
    private readonly ITravelJournalService _journalService;
    private readonly ILogger<TravelJournalsController> _logger;

    public TravelJournalsController(
        ITravelJournalService journalService,
        ILogger<TravelJournalsController> logger)
    {
        _journalService = journalService;
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

    /// <summary>
    /// Get all journals for current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TravelJournalDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TravelJournalDto>>> GetJournals()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var journals = await _journalService.GetByUserIdAsync(userId.Value);
        return Ok(journals);
    }

    /// <summary>
    /// Get journal by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TravelJournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TravelJournalDto>> GetJournal(int id)
    {
        var userId = GetUserId();
        var journal = await _journalService.GetByIdAsync(id, userId);
        if (journal == null)
        {
            return NotFound();
        }

        return Ok(journal);
    }

    /// <summary>
    /// Create new journal
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TravelJournalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TravelJournalDto>> CreateJournal([FromBody] CreateTravelJournalDto dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var journal = await _journalService.CreateAsync(dto, userId.Value);
        return CreatedAtAction(nameof(GetJournal), new { id = journal.Id }, journal);
    }

    /// <summary>
    /// Update journal
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TravelJournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TravelJournalDto>> UpdateJournal(int id, [FromBody] UpdateTravelJournalDto dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var permissions = GetPermissions();
        var journal = await _journalService.UpdateAsync(id, dto, userId.Value, permissions);
        return Ok(journal);
    }

    /// <summary>
    /// Delete journal
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteJournal(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var permissions = GetPermissions();
        await _journalService.DeleteAsync(id, userId.Value, permissions);
        return NoContent();
    }
}

