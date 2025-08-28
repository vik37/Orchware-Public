namespace Orchware.Frontoffice.API.Common.Contracts;

public interface IRedisDistributedCacheService
{
	Task<T?> GetData<T>(string key);
	Task SetData<T>(string key, T data, TimeSpan? expiry = null);
	Task RemoveData(string key);
}
