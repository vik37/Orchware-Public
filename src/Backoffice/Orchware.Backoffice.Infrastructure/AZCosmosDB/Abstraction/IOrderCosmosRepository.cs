using Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities;

namespace Orchware.Backoffice.Infrastructure.AZCosmosDB.Abstraction;

public interface IOrderCosmosRepository
{
	Task<(bool Success, string? ErrorMessage)> AddOrder(Order order);
}
