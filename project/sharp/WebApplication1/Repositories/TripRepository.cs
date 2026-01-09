using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using WebApplication1.Data;
using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using Dapper;

namespace WebApplication1.Repositories;

public class TripRepository : ITripRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<TripRepository> _logger;

    public TripRepository(
        ApplicationDbContext context,
        IDbConnection dbConnection,
        ILogger<TripRepository> logger)
    {
        _context = context;
        _dbConnection = dbConnection;
        _logger = logger;
    }

    public async Task<Trip?> GetByIdAsync(int id)
    {
        return await _context.Trips
            .Include(t => t.Location)
            .Include(t => t.TravelJournal)
                .ThenInclude(tj => tj.User)
            .Include(t => t.TripTags)
                .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<PagedResponseDto<Trip>> GetPagedAsync(int page, int pageSize, TripFilterDto? filter = null)
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            await ((NpgsqlConnection)_dbConnection).OpenAsync();
        }

        using var transaction = _dbConnection.BeginTransaction();
        try
        {
            var offset = (page - 1) * pageSize;
            var parameters = new DynamicParameters();
            var whereConditions = new List<string> { "1=1" };
            var joinConditions = new List<string>();

            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", offset);

            if (!string.IsNullOrWhiteSpace(filter?.Search))
            {
                whereConditions.Add("(t.title ILIKE @Search OR t.description ILIKE @Search OR l.name ILIKE @Search)");
                parameters.Add("Search", $"%{filter.Search}%");
            }

            if (filter?.LocationId.HasValue == true)
            {
                whereConditions.Add("t.location_id = @LocationId");
                parameters.Add("LocationId", filter.LocationId.Value);
            }

            if (filter?.TravelJournalId.HasValue == true)
            {
                whereConditions.Add("t.travel_journal_id = @TravelJournalId");
                parameters.Add("TravelJournalId", filter.TravelJournalId.Value);
            }

            if (filter?.MinRating.HasValue == true)
            {
                whereConditions.Add("t.rating >= @MinRating");
                parameters.Add("MinRating", filter.MinRating.Value);
            }

            if (filter?.StartDateFrom.HasValue == true)
            {
                whereConditions.Add("t.start_date >= @StartDateFrom");
                parameters.Add("StartDateFrom", filter.StartDateFrom.Value);
            }

            if (filter?.StartDateTo.HasValue == true)
            {
                whereConditions.Add("t.start_date <= @StartDateTo");
                parameters.Add("StartDateTo", filter.StartDateTo.Value);
            }

            var whereClause = string.Join(" AND ", whereConditions);

            var countSql = $@"
                SELECT COUNT(DISTINCT t.id)
                FROM trips t
                INNER JOIN locations l ON t.location_id = l.id
                WHERE {whereClause}";

            if (filter?.TagIds != null && filter.TagIds.Any())
            {
                countSql = $@"
                    SELECT COUNT(DISTINCT t.id)
                    FROM trips t
                    INNER JOIN locations l ON t.location_id = l.id
                    INNER JOIN trip_tags tt ON t.id = tt.trip_id
                    WHERE {whereClause} AND tt.tag_id = ANY(@TagIds)";
                parameters.Add("TagIds", filter.TagIds);
            }

            var total = await _dbConnection.QuerySingleAsync<int>(countSql, parameters, transaction);

            var sql = $@"
                SELECT DISTINCT t.id, t.travel_journal_id, t.location_id, t.title, t.description,
                       t.start_date, t.end_date, t.rating, t.photos_urls, t.created_at, t.updated_at
                FROM trips t
                INNER JOIN locations l ON t.location_id = l.id
                WHERE {whereClause}";

            if (filter?.TagIds != null && filter.TagIds.Any())
            {
                sql = $@"
                    SELECT DISTINCT t.id, t.travel_journal_id, t.location_id, t.title, t.description,
                           t.start_date, t.end_date, t.rating, t.photos_urls, t.created_at, t.updated_at
                    FROM trips t
                    INNER JOIN locations l ON t.location_id = l.id
                    INNER JOIN trip_tags tt ON t.id = tt.trip_id
                    WHERE {whereClause} AND tt.tag_id = ANY(@TagIds)";
            }

            sql += " ORDER BY t.created_at DESC LIMIT @PageSize OFFSET @Offset";

            var tripData = await _dbConnection.QueryAsync(sql, parameters, transaction);

            var tripIds = tripData.Select(t => (int)t.id).ToList();
            if (!tripIds.Any())
            {
                transaction.Commit();
                return new PagedResponseDto<Trip>
                {
                    Items = new List<Trip>(),
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                };
            }

            var trips = await _context.Trips
                .Include(t => t.Location)
                .Include(t => t.TripTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => tripIds.Contains(t.Id))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            transaction.Commit();

            _logger.LogInformation("Retrieved {Count} trips from page {Page}", trips.Count, page);

            return new PagedResponseDto<Trip>
            {
                Items = trips,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Error getting paged trips");
            throw;
        }
    }

    public async Task<Trip> CreateAsync(Trip trip)
    {
        trip.StartDate = trip.StartDate.Kind == DateTimeKind.Utc 
            ? trip.StartDate 
            : DateTime.SpecifyKind(trip.StartDate, DateTimeKind.Utc);
        if (trip.EndDate.HasValue && trip.EndDate.Value.Kind != DateTimeKind.Utc)
        {
            trip.EndDate = DateTime.SpecifyKind(trip.EndDate.Value, DateTimeKind.Utc);
        }
        trip.CreatedAt = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;
        
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created trip with id {TripId}", trip.Id);
        return trip;
    }

    public async Task<Trip> UpdateAsync(Trip trip)
    {
        trip.StartDate = trip.StartDate.Kind == DateTimeKind.Utc 
            ? trip.StartDate 
            : DateTime.SpecifyKind(trip.StartDate, DateTimeKind.Utc);
        if (trip.EndDate.HasValue && trip.EndDate.Value.Kind != DateTimeKind.Utc)
        {
            trip.EndDate = DateTime.SpecifyKind(trip.EndDate.Value, DateTimeKind.Utc);
        }
        if (trip.CreatedAt.Kind != DateTimeKind.Utc)
        {
            trip.CreatedAt = trip.CreatedAt.Kind == DateTimeKind.Local 
                ? trip.CreatedAt.ToUniversalTime() 
                : DateTime.SpecifyKind(trip.CreatedAt, DateTimeKind.Utc);
        }
        trip.UpdatedAt = DateTime.UtcNow;
        
        _context.Trips.Update(trip);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated trip with id {TripId}", trip.Id);
        return trip;
    }

    public async Task DeleteAsync(int id)
    {
        var trip = await _context.Trips.FindAsync(id);
        if (trip != null)
        {
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted trip with id {TripId}", id);
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Trips.AnyAsync(t => t.Id == id);
    }
}

