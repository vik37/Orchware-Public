using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Orchware.Backoffice.Infrastructure.Cache;

public class RedisLockService : IRedisLockService
{
	private readonly IDatabase _database;
	private readonly IConfiguration _configuration;

	public RedisLockService(IConnectionMultiplexer multiplexer, IConfiguration configuration)
	{
		_database = multiplexer.GetDatabase();
		_configuration = configuration;
	}

	public async Task<bool> TryAcquireAsync(string key, TimeSpan expiry)
	{
		var prefixedKey = $"{_configuration["RedisConnection:RedisInstance"]}:{key}";

		return await _database.StringSetAsync(
			prefixedKey,
			"1",
			expiry,
			When.NotExists);
	}

	public async Task ReleaseAsync(string key)
	{
		var prefixedKey = $"{_configuration["RedisConnection:RedisInstance"]}:{key}";
		await _database.KeyDeleteAsync(prefixedKey);
	}
}
