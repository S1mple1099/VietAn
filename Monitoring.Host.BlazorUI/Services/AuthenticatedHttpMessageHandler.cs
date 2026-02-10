namespace Monitoring.Host.BlazorUI.Services;

using Microsoft.AspNetCore.Http;

/// <summary>
/// DelegatingHandler dùng cho Blazor Server để forward auth headers/cookies từ request hiện tại
/// sang các request gọi về chính backend API (cùng host).
/// </summary>
public sealed class AuthenticatedHttpMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticatedHttpMessageHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request != null)
        {
            // Forward Authorization header
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader)
                && !request.Headers.Contains("Authorization"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", authHeader.ToString());
            }

            // Forward Cookie header (useful when auth uses cookies)
            if (httpContext.Request.Headers.TryGetValue("Cookie", out var cookieHeader)
                && !request.Headers.Contains("Cookie"))
            {
                request.Headers.TryAddWithoutValidation("Cookie", cookieHeader.ToString());
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
