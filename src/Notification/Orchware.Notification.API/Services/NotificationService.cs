using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Hubs;
using Orchware.Notification.API.Infrastructure.Persistence;
using Orchware.Notification.API.Infrastructure.Persistence.Domain;
using Orchware.Notification.API.Models;
using Orchware.Notification.API.Models.Requests;
using Orchware.Notification.API.Settings;

namespace Orchware.Notification.API.Services;

public class NotificationService : INotificationService
{
	private readonly IHubContext<NotificationHub> _hubContext;
	private readonly NotificationDbContext _dbContext;

	public NotificationService(IHubContext<NotificationHub> hubContext, NotificationDbContext dbContext)
	{
		_hubContext = hubContext;
		_dbContext = dbContext;
	}

	private async Task SaveAndSend(NotificationEntity entity, Func<IHubClients, Task> sendAction)
	{
		using var notificationActivity =
			SignalRTelemetry.ActivitySource.StartActivity(
			"notification.dispatch");

		using (SignalRTelemetry.ActivitySource.StartActivity(
			"notification.save"))
		{
			await _dbContext.Notifications.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}

		using (SignalRTelemetry.ActivitySource.StartActivity(
			"notification.signalr_send"))
		{
			await sendAction(_hubContext.Clients);
		}

		SignalRTelemetry.MessagesSent.Add(1);
	}

	public async Task BroadcastNotification(string title, string message, Guid messageId)
	{
		var entity = new NotificationEntity { Title = title, Message = message, ExternalMeesageId = messageId };
		await SaveAndSend(entity, clients =>
			clients.All.SendAsync("ReceiveNotification", entity));
	}

	public async Task SendToUser(string userId, NotificationModel model, Guid messageId)
	{
		var entity = new NotificationEntity
		{
			TargetUserId = userId,
			Title = model.Title,
			Message = model.Message,
			ActionUrl = model.ActionUrl,
			ExternalMeesageId = messageId
		};

		await SaveAndSend(entity, clients =>
			clients.User(userId).SendAsync("ReceiveNotification", entity));
	}

	public async Task SendToRole(string roleName, NotificationModel model, Guid messageId)
	{
		var entity = new NotificationEntity
		{
			TargetRole = roleName,
			Title = model.Title,
			Message = model.Message,
			ActionUrl = model.ActionUrl,
			Icon = model.Icon,
			ExternalMeesageId = messageId
		};

		await SaveAndSend(entity, clients =>
			clients.Group(roleName).SendAsync("ReceiveNotification", entity));
	}

	public async Task SendToGroup(string groupName, NotificationModel model, Guid messageId)
	{
		var entity = new NotificationEntity
		{
			TargetGroup = groupName,
			Title = model.Title,
			Message = model.Message,
			ActionUrl = model.ActionUrl,
			ExternalMeesageId = messageId
		};

		await SaveAndSend(entity, clients =>
			clients.Group(groupName).SendAsync("ReceiveNotification", entity));
	}

	public async Task<PagedResponse<List<NotificationModel>>> GetAll(PagedRequest request, Guid userId, List<string> roles)
	{
		var query = _dbContext.Notifications.AsNoTracking().AsQueryable();

		query = query.Where(x => x.TargetUserId == userId.ToString() ||
					(x.TargetRole != null && roles.Contains(x.TargetRole)) ||
					(x.TargetUserId == null && x.TargetRole == null && x.TargetGroup == null)
				);

		if (request.IsRead.HasValue)
			query = query.Where(x => x.IsRead == request.IsRead.Value);

		if (request.Filter?.Filter != null && request.Filter.Filter.Any())
		{
			foreach (var filter in request.Filter.Filter)
			{
				switch (filter.Key)
				{
					case "FromDate":
						if (DateTime.TryParse(filter.Value, out var fromDate))
							query = query.Where(x => x.CreatedAt >= fromDate.Date);
						break;
					case "ToDate":
						if (DateTime.TryParse(filter.Value, out var toDate))
							query = query.Where(x => x.CreatedAt <= toDate.Date.AddDays(1).AddTicks(-1));
						break;
				}
			}
		}

		if (!string.IsNullOrEmpty(request.Filter?.Search))
		{
			var search = request.Filter.Search.ToLower();

			query = query.Where(x => x.Title.ToLower().StartsWith(search));
		}
			

		int totalCount = await query.CountAsync();

		bool isDescending = request.Filter?.IsDescending ?? true;
			query = isDescending
				? query.OrderByDescending(x => x.CreatedAt)
				: query.OrderBy(x => x.CreatedAt);

		var data = await query
					.Skip(request.Skip)
					.Take(request.Take)
					.Select(x => new NotificationModel
					{
						Id = x.Id,
						IsRead = x.IsRead,
						ActionUrl = x.ActionUrl,
						CreatedAt = x.CreatedAt,
						Message = x.Message,
						Title = x.Title,
						Icon = x.Icon
					}).ToListAsync();

		return new PagedResponse<List<NotificationModel>>
		{
			Skip = request.Skip,
			Take = request.Take,
			Filter = request.Filter,
			Data = data,
			TotalCount = totalCount
		};
	}

	public async Task MarkAsRead(Guid id)
	{
		var entity = await _dbContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);

		if(entity is not null && !entity.IsRead)
		{
			entity.IsRead = true;
			await _dbContext.SaveChangesAsync();
		}
	}
}
