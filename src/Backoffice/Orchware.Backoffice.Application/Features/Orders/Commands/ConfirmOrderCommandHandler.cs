using MediatR;
using Orchware.Backoffice.Application.Features.Shared.CustomExceptions;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Entities.Proccessing;

namespace Orchware.Backoffice.Application.Features.Orders.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<Unit>;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Unit>
{
	private readonly IOrderRepository _orderRepository;
	private readonly IBilingTaskRepository _bilingTaskRepository;

	public ConfirmOrderCommandHandler(IOrderRepository orderRepository, IBilingTaskRepository bilingTaskRepository)
	{
		_orderRepository = orderRepository;
		_bilingTaskRepository = bilingTaskRepository;
	}

	public async Task<Unit> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			throw new OperationCanceledException();

		await _orderRepository.UnitOfWork.ExecuteAsync(async () =>
		{
			var order = await _orderRepository.GetByIdAsync(request.OrderId);

			if (order is null)
				throw new NotFoundException($"Invalid OrderID - Order was not found.");

			var orderHasRegisteredBiling = await _bilingTaskRepository.CheckIfOrderAllreadyRegistered(order.Id);
			if (orderHasRegisteredBiling)
				throw new Exception($"Order with number '{order.OrderNumber}' has allready registered Biling Process");

			if (order.Status == OrderStatus.Confirmed)
				return;

			order.SetStatus(OrderStatus.Confirmed);
			await _orderRepository.UpdateAsync(order);

			var installment = order.PaymentInfo.Installments?.FirstOrDefault();

			var billingTask = new BillingTask(
				order.Id,
				order.PaymentInfo.WillBePaidInFull,
				installment?.Id,
				order.PaymentInfo.NumberOfInstallments);

			await _bilingTaskRepository.AddBillingTask(billingTask, false);

		}, cancellationToken);

		return Unit.Value;
	}
}
