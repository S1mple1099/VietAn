namespace Monitoring.Application.DTOs.User;

public record UpdateUserRequest(
    string? FullName,
    string? Email,
    bool? IsActive,
    IList<Guid>? RoleIds);
