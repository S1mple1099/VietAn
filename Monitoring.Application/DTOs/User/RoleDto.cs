namespace Monitoring.Application.DTOs.User;

public record RoleDto(
    Guid Id,
    string Name,
    string Description,
    IList<string> Permissions);
