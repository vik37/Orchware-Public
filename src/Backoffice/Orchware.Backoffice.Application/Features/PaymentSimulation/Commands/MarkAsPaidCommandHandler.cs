using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Application.Features.PaymentSimulation.Commands;

public record MarkAsPaidCommand(string OrderNumber, string PaymentNumber, string? installmentNumber) : IRequest<Unit>;

public class MarkAsPaidCommandHandler : IRequestHandler<MarkAsPaidCommand, Unit>
{
	private readonly IOrderRepository _orderRepository;
	private readonly IIntegrationEventProducer _integrationEventProducer;

	public MarkAsPaidCommandHandler(IOrderRepository orderRepository, IIntegrationEventProducer integrationEventProducer)
	{
		_orderRepository = orderRepository;
		_integrationEventProducer = integrationEventProducer;
	}

	public async Task<Unit> Handle(MarkAsPaidCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			throw new OperationCanceledException();

		var order = await _orderRepository.GetByOrderNumber(request.OrderNumber);

		if (order is null)
			throw new NotFoundException($"Missing Order with Number: {request.OrderNumber}");

		if(order.PaymentInfo is null)
			throw new NotFoundException($"Missing Payment for Order with Number: {request.OrderNumber}");

		if(!string.IsNullOrEmpty(request.OrderNumber) && order.PaymentInfo.PaymentNumber != request.PaymentNumber)
			throw new NotFoundException($"Invalid Payment with Number {request.PaymentNumber} for Order with Number: {request.OrderNumber}");

		string? installmentCode = null;
		decimal amount = order.TotalAmount;

		if (!string.IsNullOrEmpty(request.installmentNumber) && !order.PaymentInfo.WillBePaidInFull)
		{
			installmentCode = $"{order.PaymentInfo.PaymentNumber}-{request.installmentNumber}";
			amount = order.PaymentInfo.AmountPerInstallment.HasValue ? order.PaymentInfo.AmountPerInstallment!.Value : 0;
		}

		await _integrationEventProducer.PublishPaymentRequest(order.OrderNumber, amount, order.PaymentInfo.PaymentNumber, installmentCode, $"/order/manager/{order.Id}");

		return Unit.Value;
	}
}
