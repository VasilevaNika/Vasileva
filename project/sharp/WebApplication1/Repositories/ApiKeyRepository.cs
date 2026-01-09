using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApiKeyRepository> _logger;

    public ApiKeyRepository(ApplicationDbContext context, ILogger<ApiKeyRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.KeyHash == keyHash && k.IsActive);
    }
}

