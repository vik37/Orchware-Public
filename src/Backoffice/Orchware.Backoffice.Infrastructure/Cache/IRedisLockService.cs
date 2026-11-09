namespace Orchware.Backoffice.Infrastructure.Cache;

public interface IRedisLockService
{
	Task<bool> TryAcquireAsync(string key, TimeSpan expiry);
	Task ReleaseAsync(string key);
}
