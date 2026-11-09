using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;
using Orchware.Frontoffice.API.Infrastructure.Cache.Models;

namespace Orchware.Frontoffice.API.Features.Orders.GetPendingOrder;

public class GetPendingOrderHandler : IRequestHandler<GetPendingOrderQuery, GetPendingOrderResponse?>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;

	public GetPendingOrderHandler(IRedisDistributedCacheService redisDistributedCacheService,
		IUserContextService userContextService)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
	}

	public async Task<GetPendingOrderResponse?> Handle(GetPendingOrderQuery request, CancellationToken cancellationToken)
	{
		string key = $"order:panding_user_{_userContextService.Id}";
		var order = await _redisDistributedCacheService.GetData<PendingOrderCacheModel>(key);

		if (order == null) return null;

		return MapPendingOrderCacheModelToGetPendingOrderResponse(order);
	}

	private GetPendingOrderResponse? MapPendingOrderCacheModelToGetPendingOrderResponse(PendingOrderCacheModel? model)
	{
		if(model  == null) return null;

		return new GetPendingOrderResponse
		{
			Amount = model.Amount,
			Status = new OrderStatusUIDto { Value = model.Status.Value, Color = model.Status.Color },
			Details = model.Details == null ? null : model.Details.Select(d => new OrderDetailsDto
			{
				PricePerUnit = d.PricePerUnit,
				ProductId = d.ProductId,
				Quantity = (float)d.Quantity,
				TempId = d.TempId,
				TotalPrice = d.TotalPrice,
				UnitOfMeasure = d.UnitOfMeasure,
				Product =  new ProductDto
				{
					Id = d.ProductId,
					AvailableQuantity = d.Product!.AvailableQuantity,
					MinQuantity = d.Product.MinQuantity,
					Image = d.Product.Image,
					Name = d.Product.Name,
					Price = d.Product.Price,
					SeasonalFruits = d.Product.SeasonalFruits,
					Units = d.Product.Units
				}
			}).ToList()
		};
	}
}
