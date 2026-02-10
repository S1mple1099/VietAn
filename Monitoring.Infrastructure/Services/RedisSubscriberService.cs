namespace Monitoring.Infrastructure.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

/// <summary>
/// Redis subscriber service that receives tag updates from external data collector
/// and forwards them to SignalR hub for real-time distribution to Blazor Server UI
/// </summary>
public class RedisSubscriberService : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RedisSubscriberService> _logger;

    public RedisSubscriberService(
        IConnectionMultiplexer redis,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RedisSubscriberService> logger)
    {
        _redis = redis;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();
        
        // Subscribe to tag update channel
        await subscriber.SubscribeAsync(RedisChannel.Literal("tag:updates"), async (channel, message) =>
        {
            try
            {
                var messageStr = message.ToString();
                if (string.IsNullOrEmpty(messageStr))
                {
                    return;
                }

                var tagUpdate = JsonSerializer.Deserialize<TagUpdateDto>(messageStr);
                if (tagUpdate == null)
                {
                    _logger.LogWarning("Failed to deserialize tag update: {Message}", message);
                    return;
                }

                // Create a scope to resolve scoped services
                using var scope = _serviceScopeFactory.CreateScope();
                var tagCache = scope.ServiceProvider.GetRequiredService<ITagCacheService>();
                var broadcaster = scope.ServiceProvider.GetRequiredService<IRealtimeBroadcaster>();

                // Update cache
                await tagCache.UpdateTagAsync(tagUpdate, stoppingToken);

                // Broadcast to clients via SignalR
                await broadcaster.BroadcastTagUpdateAsync(tagUpdate, stoppingToken);

                _logger.LogDebug("Tag update broadcasted: TagId={TagId}, PumpId={PumpId}", tagUpdate.TagId, tagUpdate.PumpId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing tag update from Redis");
            }
        });

        _logger.LogInformation("Redis subscriber started, listening on channel: tag:updates");

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
