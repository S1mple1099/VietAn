
namespace Monitoring.Host.Hubs;

/// <summary>
/// SignalR hub for real-time tag updates to Blazor Server UI
/// This hub runs independently from Blazor Server's internal SignalR connection
/// </summary>
[Authorize]
public class MonitorHub : Hub
{
    private readonly ITagCacheService _tagCache;
    private readonly ILogger<MonitorHub> _logger;

    public MonitorHub(ITagCacheService tagCache, ILogger<MonitorHub> logger)
    {
        _tagCache = tagCache;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        // Verify user has VIEW_MONITOR permission
        var hasPermission = Context.User?.HasClaim("permission", Permissions.ViewMonitor) ?? false;
        
        if (!hasPermission)
        {
            _logger.LogWarning("User {UserId} attempted to connect without VIEW_MONITOR permission", Context.UserIdentifier);
            Context.Abort();
            return;
        }

        _logger.LogInformation("User {UserId} connected to MonitorHub", Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogWarning(exception, "User {UserId} disconnected from MonitorHub with error", Context.UserIdentifier);
        }
        else
        {
            _logger.LogInformation("User {UserId} disconnected from MonitorHub", Context.UserIdentifier);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Client can request latest tags for a specific pump
    /// </summary>
    public async Task GetLatestTags(int pumpId)
    {
        var hasPermission = Context.User?.HasClaim("permission", Permissions.ViewMonitor) ?? false;
        if (!hasPermission)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized");
            return;
        }

        var tags = await _tagCache.GetLatestTagsAsync(pumpId);
        await Clients.Caller.SendAsync("LatestTags", tags);
    }

    /// <summary>
    /// Client can request all latest tags
    /// </summary>
    public async Task GetAllLatestTags()
    {
        var hasPermission = Context.User?.HasClaim("permission", Permissions.ViewMonitor) ?? false;
        if (!hasPermission)
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized");
            return;
        }

        var tags = await _tagCache.GetAllLatestTagsAsync();
        await Clients.Caller.SendAsync("LatestTags", tags);
    }
}
