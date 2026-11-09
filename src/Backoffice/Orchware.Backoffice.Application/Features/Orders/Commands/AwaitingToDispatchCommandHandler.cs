using MediatR;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Application.Features.Orders.Commands;

public record AwaitingToDispatchCommand(string orderNumber) : IRequest<Unit>;

public class AwaitingToDispatchCommandHandler : IRequestHandler<AwaitingToDispatchCommand, Unit>
{
	private readonly IOrderRepository _orderRepository;

	public AwaitingToDispatchCommandHandler(IOrderRepository orderRepository)
	{
		_orderRepository = orderRepository;
	}

	public async Task<Unit> Handle(AwaitingToDispatchCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			throw new OperationCanceledException();

		if (string.IsNullOrEmpty(request.orderNumber))
			throw new BadRequestException("Missing Order Number");

		var order = await _orderRepository.GetByOrderNumber(request.orderNumber);

		if (order is null)
			throw new NotFoundException($"Missing Order with number {request.orderNumber}");

		order.SetStatus(OrderStatus.AwaitingDispatch);

		await _orderRepository.UpdateAsync(order);

		await _orderRepository.UnitOfWork.SaveChangesAsync();

		return Unit.Value;
	}
}
