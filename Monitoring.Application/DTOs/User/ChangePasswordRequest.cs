namespace Monitoring.Application.DTOs.User;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);
