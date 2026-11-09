using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Domain.Events;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Orders.RequestOrder;

public class RequestOrderHandler : IRequestHandler<RequestOrderCommand, Unit>
{
	private readonly OrchwareDbContext _context;
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;
	private readonly IMediator _mediator;

	public RequestOrderHandler(OrchwareDbContext context, IRedisDistributedCacheService redisDistributedCacheService, 
		IUserContextService userContextService, IMediator mediator)
	{
		_context = context;
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
		_mediator = mediator;
	}
	public async Task<Unit> Handle(RequestOrderCommand request, CancellationToken cancellationToken)
	{
		string key = $"order:panding_user_{_userContextService.Id}";
		if (await _redisDistributedCacheService.ExistData(key))
			throw new NotFoundException("The order does not exist, or its timespan has expired");

		var paymentRules = await _context.PaymentRules.FirstOrDefaultAsync(cancellationToken);

		if (paymentRules is null)
			throw new InvalidOperationException("Payment rules are not configured.");

		if (request?.Payment?.WillBePaidInFull == false)
		{
			if (request?.Payment.NumberOfInstallments <= 0 || 
				request?.Payment.NumberOfInstallments > paymentRules.MaxAllowedInstallments)
			{
				throw new BadRequestException("Number of Installments is not allowed.");
			}
		}

		var strategy = _context.Database.CreateExecutionStrategy();

		await strategy.ExecuteAsync(async () =>
		{
			await using var tr = await _context.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				var user = await _context.User
					.Include(x => x.Company)
					.FirstOrDefaultAsync(x => x.Id == _userContextService.Id, cancellationToken);

				var orderDetails = request!.Details!.Select(x => new OrderDetails(x.ProductId,x.Quantity, x.PricePerUnit, x.UnitOfMeasure)).ToList();

				decimal amount = orderDetails.Sum(x => x.PricePerUnit);
				var order = new Order(user!, CalculateDiscount(amount, paymentRules!, request.Payment!.WillBePaidInFull));
				order.AddOrUpdateDetails(orderDetails);

				var submittedOrder = await _context.Order.AddAsync(order, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);

				var payment = new Payment(paymentRules!)
				{
					UserId = _userContextService.Id!.Value.ToString(),
					OrderId = submittedOrder.Entity.Id,
					PaymentDate = request.Payment.PaymentDate,
					WillBePaidInFull = request.Payment.WillBePaidInFull
				};

				if (!request.Payment.WillBePaidInFull)
				{
					payment.SetDetailsAndGenerateInstallments(
						order.TotalAmount,
						request.Payment.WillBePaidInFull,
						request.Payment.NumberOfInstallments,
						order.TotalAmount / request.Payment.NumberOfInstallments,
						request.Payment.PaymentDate
					);
				}

				await _context.Payment.AddAsync(payment, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);

				await tr.CommitAsync(cancellationToken);

				await _redisDistributedCacheService.RemoveData(key);

				await _mediator.Publish(
					new OrderRequestedDomainEvent(
						order.UserId, order.Discount, order.OrderNumber,
						order.Amount, order.TotalAmount,
						new PaymentDomainDto(
							payment.PaymentNumber, payment.WillBePaidInFull,
							payment.PaymentDate, payment.NumberOfInstallments,
							payment.AmountPerInstallment,
							payment.FinalInstallmentDueDate,
							payment.Installments.Select(x =>
								new InstallmentDomainDto(x.InstallmentCode, x.DueDate, x.Amount)).ToList()
						),
						request.Details!.Select(x =>
							new OrderDetailsDomainDto(
								x.Quantity, x.PricePerUnit, x.UnitOfMeasure, x.TotalPrice,
								new ProductDomainDto(
									x.Product.Name,
									x.Product.SeasonalFruits,
									x.Product.Price,
									x.Product.Units,
									x.Product.Image
								))).ToList()
					), cancellationToken);
			}
			catch
			{
				await tr.RollbackAsync(cancellationToken);
				throw;
			}
		});

		return Unit.Value;
	}


	private int CalculateDiscount(decimal amount, PaymentRules rules, bool payImmediately)
	{
		decimal discount = 0;

		if (amount >= rules.MinimumOrderAmountForDiscount)
			discount += rules.OrderAmountDiscountPercentage;

		if (payImmediately)
			discount += rules.PayImmediatelyDiscountPercentage;

		return (int)Math.Round(discount);
	}
}