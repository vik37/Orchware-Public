using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Cache.Models;

namespace Orchware.Frontoffice.API.Features.Orders.PendingOrderCreation;

public class PendingOrderCreationHandler : IRequestHandler<PendingOrderCreationCommand, Unit>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;

	public PendingOrderCreationHandler(IRedisDistributedCacheService redisDistributedCacheService, 
		IUserContextService userContextService)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
	}

	public async Task<Unit> Handle(PendingOrderCreationCommand request, CancellationToken cancellationToken)
	{
		var details = MapPendingOrderToOrderDetailsCache(request.Details).ToList();

		string key = $"order:panding_user_{_userContextService.Id}";

		var entity = await _redisDistributedCacheService.GetData<PendingOrderCacheModel>(key);

		if(entity is not null)
		{
			entity.AddOrUpdateDetails(details);
			await _redisDistributedCacheService.SetData<PendingOrderCacheModel>(key, entity, TimeSpan.FromHours(48));
			return Unit.Value;
		}

		var status = OrderStatus.Pending;
		var order = new PendingOrderCacheModel(new OrderStatusCache(status.Value, status.Color), new List<OrderDetailsCache>());
		order.AddOrUpdateDetails(details);
		await _redisDistributedCacheService.SetData<PendingOrderCacheModel>(key, order, TimeSpan.FromHours(48));
		return Unit.Value;
	}

	private IEnumerable<OrderDetailsCache> MapPendingOrderToOrderDetailsCache(IReadOnlyCollection<OrderDetailsDto> model)
	{ 
		foreach (var item in model)
		{
			var cache = new OrderDetailsCache
			{
				PricePerUnit = item.PricePerUnit,
				ProductId = item.Product == null ? 0: item.Product.Id,
				Quantity = item.Quantity,
				UnitOfMeasure = item.UnitOfMeasure,
				Product = item.Product == null ? null : new ProductCache
				{
					Id = item.Product.Id,
					AvailableQuantity = item.Product.AvailableQuantity,
					MinQuantity = item.Product.MinQuantity,
					Price = item.Product.Price,
					SeasonalFruits = item.Product.SeasonalFruits,
					Name = item.Product.Name,
					Units = item.Product.Units,
					Image = item.Product.Image
				}
			};

			cache.CalculateTotalPrice();

			yield return cache;
		}
	}
}
