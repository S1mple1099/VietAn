namespace Monitoring.Application.DTOs.Auth;

public record LoginResponse(string Token, string Username, string FullName, DateTime ExpiresAt, IList<string> Permissions);
