using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests.Repositories;

public class TripRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<ILogger<TripRepository>> _mockLogger;
    private readonly TripRepository _repository;

    public TripRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockDbConnection = new Mock<IDbConnection>();
        _mockLogger = new Mock<ILogger<TripRepository>>();
        _repository = new TripRepository(_context, _mockDbConnection.Object, _mockLogger.Object);

        SeedData();
    }

    private void SeedData()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            RoleId = 3,
            Role = new Role { Id = 3, Name = "User" }
        };

        var journal = new TravelJournal
        {
            Id = 1,
            UserId = 1,
            Title = "Test Journal",
            User = user
        };

        var location = new Location
        {
            Id = 1,
            Name = "Test Location",
            Country = "Test Country"
        };

        _context.Users.Add(user);
        _context.TravelJournals.Add(journal);
        _context.Locations.Add(location);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTrip()
    {
        var trip = new Trip
        {
            Id = 1,
            TravelJournalId = 1,
            LocationId = 1,
            Title = "Test Trip",
            StartDate = DateTime.UtcNow,
            Location = _context.Locations.First()
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test Trip");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidTrip_CreatesAndReturnsTrip()
    {
        var trip = new Trip
        {
            TravelJournalId = 1,
            LocationId = 1,
            Title = "New Trip",
            StartDate = DateTime.UtcNow
        };

        var result = await _repository.CreateAsync(trip);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Trip");

        var dbTrip = await _context.Trips.FindAsync(result.Id);
        dbTrip.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ExistingTrip_UpdatesTrip()
    {
        var trip = new Trip
        {
            Id = 1,
            TravelJournalId = 1,
            LocationId = 1,
            Title = "Original Title",
            StartDate = DateTime.UtcNow
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        trip.Title = "Updated Title";

        var result = await _repository.UpdateAsync(trip);

        result.Title.Should().Be("Updated Title");
        var dbTrip = await _context.Trips.FindAsync(1);
        dbTrip!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_ExistingTrip_RemovesTrip()
    {
        var trip = new Trip
        {
            Id = 1,
            TravelJournalId = 1,
            LocationId = 1,
            Title = "To Delete",
            StartDate = DateTime.UtcNow
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(1);

        var dbTrip = await _context.Trips.FindAsync(1);
        dbTrip.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        var trip = new Trip
        {
            Id = 1,
            TravelJournalId = 1,
            LocationId = 1,
            Title = "Test",
            StartDate = DateTime.UtcNow
        };
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingId_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(999);

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

