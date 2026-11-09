using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Infrastructure.Cache.Models;

namespace Orchware.Frontoffice.API.Features.Orders.DeletePendingOrder;

public class DeletePendingOrderHandler : IRequestHandler<DeletePendingOrderCommand, Unit>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;

	public DeletePendingOrderHandler(IRedisDistributedCacheService redisDistributedCacheService, 
		IUserContextService userContextService)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
	}

	public async Task<Unit> Handle(DeletePendingOrderCommand request, CancellationToken cancellationToken)
	{
		if(request == null) return Unit.Value;

		var isGuid = Guid.TryParse(request.TempId, out Guid tempId);
		string key = $"order:panding_user_{_userContextService.Id}";

		if (isGuid)
		{
			var order = await _redisDistributedCacheService.GetData<PendingOrderCacheModel>(key);

			if(order is not null)
			{
				order.DeleteOrderDetails(tempId);

				await _redisDistributedCacheService.SetData<PendingOrderCacheModel>(key, order);
			}
		}
		else if(string.Equals(request.TempId, "all", StringComparison.OrdinalIgnoreCase))
		{
			await _redisDistributedCacheService.RemoveData(key);
		}
		else
		{
			throw new BadRequestException("Invalid tempId format. Must be 'all' or a valid Guid.");
		}

		return Unit.Value;
	}
}
