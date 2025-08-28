using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Orchware.Frontoffice.API.Common.Contracts;

namespace Orchware.Frontoffice.API.Infrastructure.Cache;

public class RedisDistributedCacheService : IRedisDistributedCacheService
{
	private readonly IDistributedCache _distributedCache;
	private string _instance;

	public RedisDistributedCacheService(IDistributedCache distributedCache, IConfiguration configuration)
	{
		_distributedCache = distributedCache;
		_instance = configuration.GetValue<string>("RedisConnection:RedisInstance") ??
					throw new ArgumentNullException("Invalid Redis Instance: Configuration key not found.");
	}

	public async Task<T?> GetData<T>(string key)
	{
		var prefixedKey = $"{_instance}:{key}";
		var str = await _distributedCache.GetStringAsync(prefixedKey);

		if (!string.IsNullOrEmpty(str))
			return JsonSerializer.Deserialize<T>(str);

		return default(T);
	}

	public async Task SetData<T>(string key, T data, TimeSpan? expiry = null)
	{
		var prefixedKey = $"{_instance}:{key}";
		var str = JsonSerializer.Serialize(data);

		var options = new DistributedCacheEntryOptions();
		if (expiry.HasValue)
		{
			options.AbsoluteExpirationRelativeToNow = expiry.Value;
		}

		await _distributedCache.SetStringAsync(prefixedKey, str, options);
	}

	public async Task RemoveData(string key)
	{
		var prefixedKey = $"{_instance}:{key}";
		await _distributedCache.RemoveAsync(prefixedKey);
	}

}
