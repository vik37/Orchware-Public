using IntegrationEvents.Orders;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExtensions;
using Orchware.Frontoffice.API.Domain.Events;
using Orchware.Frontoffice.API.Infrastructure.Cache.Models;

using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Orders.RequestOrder;

public class RequestOrderDomainHandler : INotificationHandler<OrderRequestedDomainEvent>
{
	private readonly OrchwareDbContext _context;
	private readonly ILogger<RequestOrderDomainHandler> _logger;
	private readonly IPublishEndpoint _publishEndpointProvider;
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;

	public RequestOrderDomainHandler(OrchwareDbContext context, ILogger<RequestOrderDomainHandler> logger,
		IPublishEndpoint publishEndpointProvider, IRedisDistributedCacheService redisDistributedCacheService)
	{
		_context = context;
		_logger = logger;
		_publishEndpointProvider = publishEndpointProvider;
		_redisDistributedCacheService = redisDistributedCacheService;
	}

	public async Task Handle(OrderRequestedDomainEvent @event, CancellationToken cancellationToken)
	{
		try
		{
			var user = await _context.User.FirstOrDefaultAsync(x => x.Id == @event.UserId);

			if (user is null || user.Company is null)
			{
				_logger.LogError("{Handler} cannot process message because user {UserId} or company is missing.",
									nameof(RequestOrderDomainHandler), @event.UserId);

				throw new MissingUserOrCompanyException(@event.UserId);
			}
			
			var installments = @event.Payment.Installment == null ? null : @event.Payment.Installment
												.Select(x => new InstallmentCacheDto(
													x.InstallmentCode,
													x.DueDate,
													x.Amount))
												.ToList()??null;


			string requestedKey = $"order:requested_user_{user.Id}";
			await _redisDistributedCacheService.SetData<RequestedOrderCacheModel>(requestedKey,
					new RequestedOrderCacheModel(new UserCacheDto(user.Name, user.JobTitle, user.PersonalEmail, user.Company.Name, user.Company.Email,
													user.Company.Address, user.Company.City, user.Company.Location, user.Company.Phone, user.Company.CreatedDate),
					@event.Discount, @event.OrderNumber, @event.Amount, @event.TotalAmount,
					new PaymentCacheDto(@event.Payment.PaymentNumber, @event.Payment.WillBePaidInFull, @event.Payment.PaymentDate,
											@event.Payment.NumberOfInstallments,
											@event.Payment.AmountPerInstallment, @event.Payment.FinalInstallmentDueDate, installments), @event.OrderedDate,
					@event.OrderDetails.Select(x => new OrderDetailsCacheDto(x.Quantity, x.PricePerUnit, x.UnitOfMeasure, x.TotalPrice,
					new ProductCacheDto(x.Product.Name, x.Product.SeasonalFruits, x.Product.Price, x.Product.Units, x.Product.Image))).ToList()),
				 TimeSpan.FromHours(24));

			await _publishEndpointProvider.Publish(new OrderRequestedIntegrationEvent { OrderKey = requestedKey, 
				OrderNumber = @event.OrderNumber },cancellationToken);

			_logger.LogInformation("{Event} was finished successfully. Order {OrderNumber} published.",nameof(OrderRequestedDomainEvent), @event.OrderNumber);
		}
		catch (Exception ex)
		{
			_logger.LogError("{RequestOrderDomainHandler} failed to delivery Message. {ExceptionMessage} - {Exception}", nameof(RequestOrderDomainHandler),ex.Message, ex);
		}
	}
}