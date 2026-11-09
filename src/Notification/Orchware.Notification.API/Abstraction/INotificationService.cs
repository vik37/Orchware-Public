using Orchware.Notification.API.Models;
using Orchware.Notification.API.Models.Requests;

namespace Orchware.Notification.API.Abstraction;

public interface INotificationService
{
	#region Hub Notification
	Task BroadcastNotification(string title, string message, Guid messageId);
	Task SendToUser(string userId, NotificationModel model, Guid messageId);
	Task SendToGroup(string groupName, NotificationModel model, Guid messageId);
	Task SendToRole(string roleName, NotificationModel model, Guid messageId);
	#endregion

	#region API notification
	Task<PagedResponse<List<NotificationModel>>> GetAll(PagedRequest request, Guid userId, List<string> roles);
	Task MarkAsRead(Guid id);
	#endregion
}
