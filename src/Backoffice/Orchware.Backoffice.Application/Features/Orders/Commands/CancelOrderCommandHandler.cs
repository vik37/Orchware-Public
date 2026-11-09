using MediatR;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Enums;

namespace Orchware.Backoffice.Application.Features.Orders.Commands;

public record CancelOrderCommand(string OrderId, string Reason) : IRequest<Unit>;

public class CancleOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
	private readonly IOrderRepository _orderRepository;

	public CancleOrderCommandHandler(IOrderRepository orderRepository)
	{
		_orderRepository = orderRepository;
	}

	public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			throw new OperationCanceledException();

		bool isGuid = Guid.TryParse(request.OrderId, out Guid orderId);

		if (!isGuid)
			throw new BadRequestException("Invalid type of Order ID");

		var order = await _orderRepository.GetByIdAsync(orderId);

		if (order is null)
			throw new NotFoundException($"Missing Order with ID: {request.OrderId}");

		order.CancelOrder(request.Reason, CancelledBy.Manager);

		await _orderRepository.UpdateAsync(order);
		await _orderRepository.UnitOfWork.SaveChangesAsync();

		return Unit.Value;
	}
}
