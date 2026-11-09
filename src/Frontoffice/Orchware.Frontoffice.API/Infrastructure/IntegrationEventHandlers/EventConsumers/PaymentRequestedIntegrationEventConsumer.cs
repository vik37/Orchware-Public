using IntegrationEvents.IntegrationEvents.Payment;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;

public class PaymentRequestedIntegrationEventConsumer : IConsumer<PaymentRequestedIntegrationEvent>
{
	private readonly OrchwareDbContext _dbContext;
	private readonly ILogger<OrderPackedIntegrationEventConsumer> _logger;
	private readonly IRedisLockService _redisLockService;

	public PaymentRequestedIntegrationEventConsumer(OrchwareDbContext dbContext, IRedisLockService redisLockService,
		ILogger<OrderPackedIntegrationEventConsumer> logger)
	{
		_dbContext = dbContext;
		_redisLockService = redisLockService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<PaymentRequestedIntegrationEvent> context)
	{
		var message = context.Message;

		var acquired = await _redisLockService.TryAcquireAsync(
			message.MessageId.ToString(),
			TimeSpan.FromHours(1));

		if (!acquired)
			return;

		var order = await _dbContext.Order
			.Include(x => x.OrderDetails)
			.Include(x => x.Company)
			.FirstOrDefaultAsync(x => x.OrderNumber == message.OrderNumber);

		if (order == null)
		{
			_logger.LogError(
				"Order {OrderNumber} not found",
				message.OrderNumber);

			await _redisLockService.ReleaseAsync(
				message.MessageId.ToString());

			return;
		}

		var payment = await _dbContext.Payment
			.Include(x => x.Installments)
			.FirstOrDefaultAsync(x => x.OrderId == order.Id);

		if (payment == null)
		{
			_logger.LogError(
				"Payment not found for order {OrderNumber}",
				order.OrderNumber);

			await _redisLockService.ReleaseAsync(
				message.MessageId.ToString());

			return;
		}

		if (!Guid.TryParse(payment.UserId, out Guid userId))
		{
			_logger.LogError(
				"Invalid USER ID for order {OrderNumber}",
				order.OrderNumber);

			await _redisLockService.ReleaseAsync(
				message.MessageId.ToString());

			return;
		}

		var user = await _dbContext.User
			.FirstOrDefaultAsync(x => x.Id == userId);

		if (user == null)
		{
			_logger.LogError(
				"User not found for order {OrderNumber}",
				order.OrderNumber);

			await _redisLockService.ReleaseAsync(
				message.MessageId.ToString());

			return;
		}

		if (order.Company.Budget < message.Amount)
		{
			_logger.LogWarning(
				"Insufficient budget for Order: {OrderNumber}",
				order.OrderNumber);

			var paymentRule = await _dbContext.PaymentRules
				.FirstOrDefaultAsync();

			DateTime? baseDate = payment.WillBePaidInFull
				? payment.PaidInFullDate
				: payment.FinalInstallmentDueDate;

			await context.Publish(new PaymentRejectedIntegrationEvent
			{
				Title = $"[ALERT] Insufficient budget for order: {order.OrderNumber}",

				Message =
					$"Order <b>[{order.OrderNumber}]</b> cannot currently be finalized due to insufficient available budget.",

				ActionURL = message.actionUrl!,

				Customer = user.Name,

				CustomerEmail = user.PersonalEmail,

				PaymentDueDate = baseDate?.Add(
					paymentRule?.GracePeriod ?? TimeSpan.Zero)
			});

			return;
		}

		var strategy = _dbContext.Database.CreateExecutionStrategy();

		await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				order.Company.Budget -= message.Amount;

				foreach (var detail in order.OrderDetails)
				{
					var product = await _dbContext.Product
						.FindAsync(detail.ProductId);

					if (product != null)
					{
						decimal amountToReduce =
							UnitConverter.Convert(
								detail.Quantity,
								detail.UnitOfMeasure,
								product.Units.GetDisplayNameToString());

						product.AvailableQuantity -=
							(float)amountToReduce;
					}
				}

				if (!string.IsNullOrEmpty(message.InstallmentCode))
				{
					payment.MarkInstallmentAsPaid(
						message.InstallmentCode);
				}
				else
				{
					payment.MarkAsPaid(
						DateTime.UtcNow);
				}

				await _dbContext.SaveChangesAsync(
					context.CancellationToken);

				await transaction.CommitAsync();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				await _redisLockService.ReleaseAsync(message.MessageId.ToString());

				_logger.LogError(
					ex,
					"Failed to finalize order {OrderNumber} after payment.",
					order.OrderNumber);

				throw;
			}
		});

		await context.Publish(new PaymentConfirmedIntegrationEvent
		{
			OrderNumber = order.OrderNumber,
			PaymentNumber = payment.PaymentNumber,
			InstallmentCode = message.InstallmentCode,
			Customer = user.Name,
			CustomerEmail = user.PersonalEmail,
			ActionUrl = message.actionUrl!
		});
	}
}
