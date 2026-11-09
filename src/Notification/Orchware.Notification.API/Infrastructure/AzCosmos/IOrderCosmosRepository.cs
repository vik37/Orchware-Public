using Orchware.Notification.API.Infrastructure.AzCosmos.Entity;

namespace Orchware.Notification.API.Infrastructure.AzCosmos;

public interface IOrderCosmosRepository
{
	Task<Order?> GetOrderByOrderNumber(string orderNumber);
	Task MarkEmailAsSent(string id, string orderNumber);
	Task ResetEmailStatus(string id, string orderNumber);
	Task DeleteOrderAsync(string id, string orderNumber);
}
