using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Orchware.Notification.API.Infrastructure.AzCosmos.Entity;
using Orchware.Notification.API.Settings.Models;

namespace Orchware.Notification.API.Infrastructure.AzCosmos;

public class OrderCosmosRepository : IOrderCosmosRepository
{
	private readonly Container _container;
	private readonly ILogger<OrderCosmosRepository> _logger;

	public OrderCosmosRepository(CosmosClient client, IOptions<CosmosDBSettings> options, ILogger<OrderCosmosRepository> logger)
	{
		_container = client.GetDatabase(options.Value.Database).GetContainer("order");
		_logger = logger;
	}

	public async Task<Order?> GetOrderByOrderNumber(string orderNumber)
	{
		try
		{
			var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.orderNumber = @orderNumber")
				.WithParameter("@orderNumber", orderNumber);

			var queryOptions = new QueryRequestOptions
			{
				PartitionKey = new PartitionKey(orderNumber)
			};

			using var iterator = _container.GetItemQueryIterator<Order>(queryDefinition, requestOptions: queryOptions);

			if (iterator.HasMoreResults)
			{
				var response = await iterator.ReadNextAsync();
				return response.FirstOrDefault();
			}

			return null;
		}
		catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			return null;
		}
	}

	public async Task MarkEmailAsSent(string id, string orderNumber)
	{
		var options = new PatchItemRequestOptions
		{
			FilterPredicate = "from c where c.invoice.emailSent = false OR NOT IS_DEFINED(c.invoice.emailSent)"
		};

		await _container.PatchItemAsync<Order>(
			id: id,
			partitionKey: new PartitionKey(orderNumber),
			patchOperations: new[]
			{
				PatchOperation.Set("/invoice/emailSent", true),
				PatchOperation.Set("/invoice/emailSentAt", DateTime.UtcNow)
			},
			requestOptions: options);
	}

	public async Task ResetEmailStatus(string id, string orderNumber)
	{
		try
		{
			await _container.PatchItemAsync<dynamic>(
				id: id,
				partitionKey: new PartitionKey(orderNumber),
				patchOperations: new[]
				{
				PatchOperation.Set("/invoice/emailSent", false),
				PatchOperation.Set("/invoice/emailSentAt", (DateTime?)null)
				});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to rollback email status for order {Num}", orderNumber);
		}
	}

	public async Task DeleteOrderAsync(string id, string orderNumber)
	{
		await _container.DeleteItemAsync<Order>(
			id: id,
			partitionKey: new PartitionKey(orderNumber)
		);
	}
}
