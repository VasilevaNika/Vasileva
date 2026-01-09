using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests.Repositories;

public class TravelJournalRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<TravelJournalRepository>> _mockLogger;
    private readonly TravelJournalRepository _repository;

    public TravelJournalRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<TravelJournalRepository>>();
        _repository = new TravelJournalRepository(_context, _mockLogger.Object);

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

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_ValidJournal_CreatesAndReturnsJournal()
    {
        var journal = new TravelJournal
        {
            UserId = 1,
            Title = "New Journal",
            Description = "Test Description"
        };

        var result = await _repository.CreateAsync(journal);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Journal");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsJournal()
    {
        var journal = new TravelJournal
        {
            Id = 1,
            UserId = 1,
            Title = "Test Journal"
        };
        _context.TravelJournals.Add(journal);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test Journal");
    }

    [Fact]
    public async Task UpdateAsync_ExistingJournal_UpdatesJournal()
    {
        var journal = new TravelJournal
        {
            Id = 1,
            UserId = 1,
            Title = "Original Title"
        };
        _context.TravelJournals.Add(journal);
        await _context.SaveChangesAsync();

        journal.Title = "Updated Title";

        var result = await _repository.UpdateAsync(journal);

        result.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_ExistingJournal_RemovesJournal()
    {
        var journal = new TravelJournal
        {
            Id = 1,
            UserId = 1,
            Title = "To Delete"
        };
        _context.TravelJournals.Add(journal);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(1);

        var dbJournal = await _context.TravelJournals.FindAsync(1);
        dbJournal.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAsync_ExistingUserId_ReturnsJournals()
    {
        var journal1 = new TravelJournal { Id = 1, UserId = 1, Title = "Journal 1" };
        var journal2 = new TravelJournal { Id = 2, UserId = 1, Title = "Journal 2" };
        _context.TravelJournals.AddRange(journal1, journal2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserIdAsync(1);

        result.Should().HaveCount(2);
        result.Should().Contain(j => j.Title == "Journal 1");
        result.Should().Contain(j => j.Title == "Journal 2");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

