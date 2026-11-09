using MassTransit;
using Orchware.Backoffice.Infrastructure.Cache;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.Filter;

public class IdempotencyFilter<T> : IFilter<ConsumeContext<T>>
	where T : class
{
	private readonly IRedisLockService _redisLockService;

	public IdempotencyFilter(IRedisLockService redisLockService)
	{
		_redisLockService = redisLockService;
	}

	public async Task Send(
		ConsumeContext<T> context,
		IPipe<ConsumeContext<T>> next)
	{
		var messageId = context.MessageId?.ToString();

		if (string.IsNullOrWhiteSpace(messageId))
		{
			await next.Send(context);
			return;
		}

		var acquired = await _redisLockService.TryAcquireAsync(
			messageId,
			TimeSpan.FromHours(1));

		if (!acquired)
			return;

		try
		{
			await next.Send(context);
		}
		catch
		{
			await _redisLockService.ReleaseAsync(messageId);
			throw;
		}
	}

	public void Probe(ProbeContext context)
	{
		context.CreateFilterScope("redis-idempotency");
	}
}
