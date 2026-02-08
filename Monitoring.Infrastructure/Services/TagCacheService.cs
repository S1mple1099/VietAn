
namespace Monitoring.Infrastructure.Services;

using StackExchange.Redis;

/// <summary>
/// Redis-based tag cache service for real-time tag values
/// </summary>
public class TagCacheService : ITagCacheService
{
    private readonly IDatabase _redis;
    private const string TagKeyPrefix = "tag:";
    private const string PumpTagsKeyPrefix = "pump:tags:";

    public TagCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task UpdateTagAsync(TagUpdateDto tagUpdate, CancellationToken cancellationToken = default)
    {
        var key = $"{TagKeyPrefix}{tagUpdate.TagId}";
        var pumpKey = $"{PumpTagsKeyPrefix}{tagUpdate.PumpId}";

        var json = JsonSerializer.Serialize(tagUpdate);
        await _redis.StringSetAsync(key, json, TimeSpan.FromHours(24));
        await _redis.SetAddAsync(pumpKey, tagUpdate.TagId.ToString());
        await _redis.KeyExpireAsync(pumpKey, TimeSpan.FromHours(24));
    }

    public async Task<IEnumerable<TagUpdateDto>> GetLatestTagsAsync(int pumpId, CancellationToken cancellationToken = default)
    {
        var pumpKey = $"{PumpTagsKeyPrefix}{pumpId}";
        var tagIds = await _redis.SetMembersAsync(pumpKey);

        if (tagIds.Length == 0)
        {
            return Enumerable.Empty<TagUpdateDto>();
        }

        var tasks = tagIds.Select(async tagId =>
        {
            var key = $"{TagKeyPrefix}{tagId}";
            var json = await _redis.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<TagUpdateDto>(json!);
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null)!;
    }

    public async Task<IEnumerable<TagUpdateDto>> GetAllLatestTagsAsync(CancellationToken cancellationToken = default)
    {
        // Get all tag keys
        var server = _redis.Multiplexer.GetServer(_redis.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{TagKeyPrefix}*");

        var tasks = keys.Select(async key =>
        {
            var json = await _redis.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<TagUpdateDto>(json!);
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null)!;
    }
}
