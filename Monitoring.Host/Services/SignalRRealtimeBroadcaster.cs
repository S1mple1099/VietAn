namespace Monitoring.Host.Services;

using Monitoring.Application.DTOs.Monitor;
using Monitoring.Application.Interfaces;
using Monitoring.Host.Hubs;

/// <summary>
/// SignalR implementation of IRealtimeBroadcaster
/// Bridges Application layer interface with Host layer SignalR hub
/// </summary>
public class SignalRRealtimeBroadcaster : IRealtimeBroadcaster
{
    private readonly IHubContext<MonitorHub> _hubContext;

    public SignalRRealtimeBroadcaster(IHubContext<MonitorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastTagUpdateAsync(TagUpdateDto tagUpdate, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("TagUpdate", tagUpdate, cancellationToken);
    }
}
