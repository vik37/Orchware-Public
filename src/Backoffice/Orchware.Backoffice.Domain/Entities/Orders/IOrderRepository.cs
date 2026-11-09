using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public interface IOrderRepository : IRepository<Order, Guid>
{
	Task<Order?> GetByOrderNumber(string number, CancellationToken cancellationToken = default);
}
