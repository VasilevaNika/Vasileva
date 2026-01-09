using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests.Repositories;

public class LocationRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<LocationRepository>> _mockLogger;
    private readonly LocationRepository _repository;

    public LocationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<LocationRepository>>();
        _repository = new LocationRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidLocation_CreatesAndReturnsLocation()
    {
        var location = new Location
        {
            Name = "Test Location",
            Country = "Test Country",
            City = "Test City"
        };

        var result = await _repository.CreateAsync(location);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Location");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsLocation()
    {
        var location = new Location
        {
            Id = 1,
            Name = "Test Location",
            Country = "Test Country"
        };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test Location");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllLocations()
    {
        var location1 = new Location { Id = 1, Name = "Location A", Country = "Country" };
        var location2 = new Location { Id = 2, Name = "Location B", Country = "Country" };
        _context.Locations.AddRange(location1, location2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(l => l.Name == "Location A");
        result.Should().Contain(l => l.Name == "Location B");
    }

    [Fact]
    public async Task UpdateAsync_ExistingLocation_UpdatesLocation()
    {
        var location = new Location
        {
            Id = 1,
            Name = "Original Name",
            Country = "Country"
        };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        location.Name = "Updated Name";

        var result = await _repository.UpdateAsync(location);

        result.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ExistingLocation_RemovesLocation()
    {
        var location = new Location
        {
            Id = 1,
            Name = "To Delete",
            Country = "Country"
        };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(1);

        var dbLocation = await _context.Locations.FindAsync(1);
        dbLocation.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

