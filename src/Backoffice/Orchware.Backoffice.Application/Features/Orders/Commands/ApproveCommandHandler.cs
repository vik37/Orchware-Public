using MediatR;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Application.Features.Orders.Commands;

public record ApproveCommand(string orderId) : IRequest<Unit>;

public class ApproveCommandHandler : IRequestHandler<ApproveCommand, Unit>
{
	private readonly IOrderRepository _orderRepository;

	public ApproveCommandHandler(IOrderRepository orderRepository)
	{
		_orderRepository = orderRepository;
	}

	public async Task<Unit> Handle(ApproveCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			throw new OperationCanceledException();

		bool isGuid = Guid.TryParse(request.orderId, out Guid orderId);

		if (!isGuid)
			throw new BadRequestException("Invalid type of Order ID");

		var order = await _orderRepository.GetByIdAsync(orderId);

		if(order is null)
			throw new NotFoundException($"Missing Order with ID: {request.orderId}");

		order.SetStatus(OrderStatus.Approved);

		await _orderRepository.UpdateAsync(order);
		await _orderRepository.UnitOfWork.SaveChangesAsync();

		return Unit.Value;
	}
}
