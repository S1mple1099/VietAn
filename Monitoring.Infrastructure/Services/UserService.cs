namespace Monitoring.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly MonitoringDbContext _context;

    public UserService(MonitoringDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserDto(
            Id: u.Id,
            Username: u.Username,
            FullName: u.FullName,
            Email: u.Email,
            IsActive: u.IsActive,
            CreatedAt: u.CreatedAt,
            LastLoginAt: u.LastLoginAt,
            Roles: u.UserRoles.Select(ur => ur.Role.Name).ToList()
        ));
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return null;

        return new UserDto(
            Id: user.Id,
            Username: user.Username,
            FullName: user.FullName,
            Email: user.Email,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt,
            LastLoginAt: user.LastLoginAt,
            Roles: user.UserRoles.Select(ur => ur.Role.Name).ToList()
        );
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken))
        {
            throw new InvalidOperationException($"Username '{request.Username}' already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = Infrastructure.Services.AuthService.HashPassword(request.Password),
            FullName = request.FullName,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Assign roles
        if (request.RoleIds.Any())
        {
            var roles = await _context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            foreach (var role in roles)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetUserByIdAsync(user.Id, cancellationToken) 
            ?? throw new InvalidOperationException("Failed to retrieve created user");
    }

    public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new InvalidOperationException($"User with ID '{userId}' not found");

        if (request.FullName != null)
            user.FullName = request.FullName;

        if (request.Email != null)
            user.Email = request.Email;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        // Update roles if provided
        if (request.RoleIds != null)
        {
            // Remove existing roles
            _context.UserRoles.RemoveRange(user.UserRoles);

            // Add new roles
            var roles = await _context.Roles
                .Where(r => request.RoleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            foreach (var role in roles)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await GetUserByIdAsync(user.Id, cancellationToken) 
            ?? throw new InvalidOperationException("Failed to retrieve updated user");
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([userId], cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{userId}' not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync([userId], cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{userId}' not found");

        // Verify current password
        if (!Infrastructure.Services.AuthService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        user.PasswordHash = Infrastructure.Services.AuthService.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);

        return roles.Select(r => new RoleDto(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Permissions: r.RolePermissions.Select(rp => rp.Permission.Name).ToList()
        ));
    }

    public async Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null)
            return null;

        return new RoleDto(
            Id: role.Id,
            Name: role.Name,
            Description: role.Description,
            Permissions: role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
        );
    }
}
