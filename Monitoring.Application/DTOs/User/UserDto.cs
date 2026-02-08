namespace Monitoring.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string Username,
    string FullName,
    string Email,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    IList<string> Roles);
