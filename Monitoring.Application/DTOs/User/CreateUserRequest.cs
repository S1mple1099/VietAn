namespace Monitoring.Application.DTOs.User;

public record CreateUserRequest(
    string Username,
    string Password,
    string FullName,
    string Email,
    IList<Guid> RoleIds);
