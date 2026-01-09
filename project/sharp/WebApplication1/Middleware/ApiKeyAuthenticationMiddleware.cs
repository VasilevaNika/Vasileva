using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Middleware;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyRepository apiKeyRepository)
        : base(options, logger, encoder)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-API-KEY", out StringValues apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var apiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        // Hash the API key (in production, use proper hashing)
        var keyHash = ComputeHash(apiKey);
        var apiKeyEntity = await _apiKeyRepository.GetByKeyHashAsync(keyHash);

        if (apiKeyEntity == null)
        {
            return AuthenticateResult.Fail("Invalid API key");
        }

        if (apiKeyEntity.ExpiresAt.HasValue && apiKeyEntity.ExpiresAt.Value < DateTime.UtcNow)
        {
            return AuthenticateResult.Fail("API key has expired");
        }

        if (!apiKeyEntity.IsActive)
        {
            return AuthenticateResult.Fail("API key is inactive");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "api-key"),
            new Claim(ClaimTypes.Name, apiKeyEntity.Name),
            new Claim(ClaimTypes.Role, "ApiKey"),
            new Claim("permissions", "trips.read,journals.read,locations.read")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private string ComputeHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashedBytes);
    }
}

