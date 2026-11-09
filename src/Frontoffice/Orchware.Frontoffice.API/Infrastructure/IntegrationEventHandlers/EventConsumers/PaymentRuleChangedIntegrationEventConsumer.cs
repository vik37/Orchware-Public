using IntegrationEvents.IntegrationEvents.PaymentRules;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class PaymentRuleChangedIntegrationEventConsumer : IConsumer<PaymentRuleChangedIntegrationEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<OrderPackedIntegrationEventConsumer> _logger;
	private readonly IRedisLockService _redisLockService;

	public PaymentRuleChangedIntegrationEventConsumer(OrchwareDbContext dbContext, IRedisLockService redisLockService,
		ILogger<OrderPackedIntegrationEventConsumer> logger)
	{
		_dbContext = dbContext;
		_redisLockService = redisLockService;
		_logger = logger;
	}
	public async Task Consume(ConsumeContext<PaymentRuleChangedIntegrationEvent> context)
	{
		var message = context.Message;
		var lockKey = message.MessageId.ToString();

		var acquired = await _redisLockService.TryAcquireAsync(lockKey, TimeSpan.FromHours(1));

		if (!acquired)
			return;

		try
		{
			var rule = await _dbContext.PaymentRules.FirstOrDefaultAsync();

			if (rule is null)
			{
				_logger.LogError("Critical: PaymentRule configuration record is missing in the database while processing event {MessageId}. Technical intervention required.", message.MessageId);
				throw new InvalidOperationException($"Payment rules are missing. Cannot process event {message.MessageId}.");
			}

			rule.MaxAllowedInstallments = message.MaxAllowedInstallments;
			rule.GracePeriod = message.GracePeriod;
			rule.MaximumPaymentPeriod = message.MaximumPaymentPeriod;
			rule.MinimumOrderAmountForDiscount = message.MinimumOrderAmountForDiscount;
			rule.OrderAmountDiscountPercentage = message.OrderAmountDiscountPercentage;
			rule.PayImmediatelyDiscountPercentage = message.PayImmediatelyDiscountPercentage;

			await _dbContext.SaveChangesAsync();
		}
		finally
		{
			await _redisLockService.ReleaseAsync(lockKey); 
		}
	}
}
