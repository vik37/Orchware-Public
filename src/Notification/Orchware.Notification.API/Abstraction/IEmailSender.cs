using Orchware.Notification.API.Models;

namespace Orchware.Notification.API.Abstraction;

public interface INotifySender
{
	Task SendEmailAsync(EmailModel model);
}
