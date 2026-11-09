namespace Orchware.Frontoffice.API.Common.Contracts;

public interface IRedisLockService
{
	Task<bool> TryAcquireAsync(string key, TimeSpan expiry);
	Task ReleaseAsync(string key);
}
