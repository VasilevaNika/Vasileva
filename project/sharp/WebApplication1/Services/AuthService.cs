using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for username {Username}", dto.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive");
        }

        var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);
        var token = GenerateJwtToken(user, permissions);

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role.Name,
            Permissions = permissions
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            var roleName = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role.Trim();
            _logger.LogInformation("Registering user {Username} with requested role: {RoleName}", dto.Username, roleName);
            
            var userRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());
            
            if (userRole == null)
            {
                _logger.LogError("Role {RoleName} not found in database", roleName);
                var availableRoles = await _context.Roles.Select(r => r.Name).ToListAsync();
                throw new InvalidOperationException($"Role '{roleName}' is not configured in the system. Available roles: {string.Join(", ", availableRoles)}");
            }
            
            _logger.LogInformation("Found role {RoleName} with ID {RoleId} for user {Username}", userRole.Name, userRole.Id, dto.Username);

            var passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                RoleId = userRole.Id,
                IsActive = true
            };

            _logger.LogInformation("Creating user {Username} with RoleId: {RoleId} (Role: {RoleName})", 
                dto.Username, userRole.Id, userRole.Name);
            
            user = await _userRepository.CreateAsync(user);
            var userId = user.Id;
            
            _logger.LogInformation("User created with ID: {UserId}, RoleId after creation: {RoleId}", 
                userId, user.RoleId);
            
            if (user.RoleId != userRole.Id)
            {
                _logger.LogWarning("RoleId mismatch! Expected: {ExpectedRoleId}, Actual: {ActualRoleId}. Fixing...", 
                    userRole.Id, user.RoleId);
                user.RoleId = userRole.Id;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            
            // Reload user with Role navigation property included
            user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("Failed to retrieve created user with ID {UserId}", userId);
                throw new InvalidOperationException("Failed to retrieve created user");
            }
            
            if (user.Role == null)
            {
                _logger.LogError("User role is null for user ID {UserId}", user.Id);
                throw new InvalidOperationException("User role was not loaded correctly");
            }
            
            _logger.LogInformation("User {UserId} loaded with role: {RoleName} (RoleId: {RoleId})", 
                user.Id, user.Role.Name, user.Role.Id);
            
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);
            var token = GenerateJwtToken(user, permissions);

            _logger.LogInformation("User {UserId} registered successfully", user.Id);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role.Name,
                Permissions = permissions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for username {Username}", dto.Username);
            throw;
        }
    }

    private string GenerateJwtToken(User user, List<string> permissions)
    {
        var secretKey = _configuration["JWT:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            _logger.LogError("JWT SecretKey is not configured");
            throw new InvalidOperationException("JWT configuration is missing");
        }

        if (user.Role == null)
        {
            _logger.LogError("User role is null when generating token for user ID {UserId}", user.Id);
            throw new InvalidOperationException("User role is required to generate token");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("permissions", string.Join(",", permissions))
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _configuration["JWT:Issuer"] ?? "TravelJournalAPI",
            Audience = _configuration["JWT:Audience"] ?? "TravelJournalUsers",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        // Simple SHA256 with salt (in production use BCrypt.Net-Next)
        using var sha256 = SHA256.Create();
        var salt = "TravelJournalSalt2024";
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }
}

