using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Abstraction;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities;
using Orchware.Backoffice.Infrastructure.AZCosmosDB.Settings;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Orchware.Backoffice.Infrastructure.AZCosmosDB;

public class OrderCosmosRepository : IOrderCosmosRepository
{
	private readonly Container _container;

	public OrderCosmosRepository(CosmosClient client, IOptions<CosmosDBSettings> options)
	{
		_container = client.GetDatabase(options.Value.Database).GetContainer("order");
	}

	public async Task<(bool Success, string? ErrorMessage)> AddOrder(Order order)
	{
		try
		{
			await _container.CreateItemAsync(
				order,
				new PartitionKey(order.OrderNumber)
			);

			return (true, null);
		}
		catch(CosmosException ex)
		{
			string error = ex.Message;
			if (ex.InnerException != null)
				error += " | Inner: " + ex.InnerException.Message;

			return (false, error);
		}
		catch (Exception ex)
		{
			string error = ex.Message;
			if (ex.InnerException != null)
				error += " | Inner: " + ex.InnerException.Message;

			return (false, error);
		}
	}
}
