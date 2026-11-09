using IntegrationEvents.IntegrationEvents.Notification;
using IntegrationEvents.Orders;
using MassTransit;
using Microsoft.Extensions.Logging;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Primitives;
using Orchware.Backoffice.Infrastructure.Cache.Models;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.EventConsumers;

public class OrderRequestedIntegrationEventConsumer : IConsumer<OrderRequestedIntegrationEvent>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly ILogger<OrderRequestedIntegrationEventConsumer> _logger;
	private readonly IOrderRepository _orderRepository;

	public OrderRequestedIntegrationEventConsumer(IRedisDistributedCacheService redisDistributedCacheService,
		ILogger<OrderRequestedIntegrationEventConsumer> logger,
		IOrderRepository orderRepository)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_logger = logger;
		_orderRepository = orderRepository;
	}

	public async Task Consume(ConsumeContext<OrderRequestedIntegrationEvent> context)
	{
		var order = await _redisDistributedCacheService.GetData<RequestedOrderCacheModel>(context.Message.OrderKey);

		if (order is null || order.OrderNumber != context.Message.OrderNumber)
		{
			_logger.LogWarning(
					"Invalid or missing order. Skipping event {EventId}",
					context.Message.OrderKey);
			return;
		}

		var existingOrder = await _orderRepository.GetByOrderNumber(context.Message.OrderNumber);

		if (existingOrder is not null)
		{
			_logger.LogWarning(
					"Duplicate Order. Order {OrderNumber} allready exist. Skipping event {EventId}",
					context.Message.OrderNumber,context.Message.OrderKey);
			return;
		}

		var installments = order.Payment.Installments == null ? null : order.Payment.Installments
												.Select(x => new InstallmentCacheDto(
													x.InstallmentCode,
													x.DueDate,
													x.Amount))
												.ToList() ?? null;

		var entity = new Order(order!.OrderNumber, order.OrderedDate, order.Amount, order.TotalAmount, order.Discount,
				new PaymentInfo(order.Payment.PaymentNumber, order.Payment.WillBePaidInFull, order.Payment.PaymentDate,
					installments?.Select(i => new Installment(i.DueDate, i.Amount, i.InstallmentCode)).ToList(),
																 order.Payment?.NumberOfInstallments, order.Payment?.AmountPerInstallment,
																 order.Payment?.FinalInstallmentDueDate),
					new Buyer(order.User.Name, order.User.JobTitle, order.User.PersonalEmail, order.User.CompanyName, order.User.CompanyEmail,
								order.User.CompanyAddress, order.User.CompanyCity, order.User.CompanyLocation, order.User.CompanyPhoneNumber,
								order.User.CompanyRegisteredDate),
					order.OrderDetails.Select(od => new OrderDetails(new OrderedProduct
					{
						Name = od.Product.Name,
						SeasonalFruits = od.Product.SeasonalFruits,
						Price = od.Product.Price,
						Units = od.Product.Units,
						Image = od.Product.Image
					},
					od.Quantity, od.PricePerUnit, od.UnitOfMeasure, od.TotalPrice)
				));

		await _orderRepository.AddAsync(entity);

		entity.AddRequestedOrderDomainEvent();

		await _orderRepository.UnitOfWork.SaveChangesAsync();

		await _redisDistributedCacheService.RemoveData(context.Message.OrderKey);

		_logger.LogInformation("Order {OrderNumber} successfully processed {Event}",
			entity.OrderNumber, nameof(OrderRequestedIntegrationEvent));

		await context.Publish(new ManagerNotificationMessageIntegrationEvent
		{
			Title = "You have a request for a new order",
			Message = $"There is a request for a new order with a number {order.OrderNumber} from a customer {order.User.Name}, for the company {order.User.CompanyName}",
			OrderNumber = entity.OrderNumber,
			ActionUrl = $"/order/manager/{entity.Id}",
			Icon = "receipt"
		});
	}
}
