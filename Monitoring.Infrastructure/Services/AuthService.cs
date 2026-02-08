
namespace Monitoring.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly MonitoringDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IAuditService _auditService;

    public AuthService(MonitoringDbContext context, IConfiguration configuration, IAuditService auditService)
    {
        _context = context;
        _configuration = configuration;
        _auditService = auditService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, string ipAddress, string userAgent, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive, cancellationToken);

        if (user == null)
        {
            await _auditService.LogLoginAsync(Guid.Empty, request.Username, ipAddress, userAgent, false, "User not found", cancellationToken);
            return null;
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            await _auditService.LogLoginAsync(user.Id, request.Username, ipAddress, userAgent, false, "Invalid password", cancellationToken);
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Log successful login
        await _auditService.LogLoginAsync(user.Id, request.Username, ipAddress, userAgent, true, null, cancellationToken);

        // Get permissions
        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);

        // Generate JWT
        var token = GenerateJwtToken(user, permissions);
        var expiresAt = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("Jwt:ExpirationHours", 8));

        return new LoginResponse(token, user.Username, user.FullName, expiresAt, permissions);
    }

    public async Task<IList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var permissions = await _context.Users
            .Where(u => u.Id == userId && u.IsActive)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissions;
    }

    private string GenerateJwtToken(User user, IList<string> permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationHours = _configuration.GetValue<int>("Jwt:ExpirationHours", 8);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add permission claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "MonitoringSystem",
            audience: _configuration["Jwt:Audience"] ?? "MonitoringSystem",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        // Simple hash verification - in production, use BCrypt or Argon2
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashString = Convert.ToBase64String(hashedBytes);
        return hashString == hash;
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
