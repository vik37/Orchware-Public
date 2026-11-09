using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchware.Notification.API.Abstraction;
using Orchware.Notification.API.Filters;
using Orchware.Notification.API.Models.Requests;
using System.Security.Claims;

namespace Orchware.Notification.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
	private readonly INotificationService _notificationService;

	public NotificationController(INotificationService notificationService)
	{
		_notificationService = notificationService;
	}

	[HttpPost("test")]
	public async Task Test(string message)
	{
		await _notificationService.BroadcastNotification("Broadcast Message: ", message, Guid.NewGuid());
	}

	[HttpPost]
	[Authorize]
	[ValidateNotificationRequestAttribute]
	public async Task<IActionResult> GetNotifications(PagedRequest request)
	{
		var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
		Guid userId = Guid.Empty;
		if (!string.IsNullOrEmpty(userIdRaw))
		{
			Guid.TryParse(userIdRaw, out userId);
		}

		var allRoles = User.FindAll(ClaimTypes.Role).Select(x => x.Value)
													.Concat(User.FindAll("role").Select(x => x.Value))
													.ToList();

		var businessRoles = new[] { "warehouseman", "manager" };

		var roles = allRoles
			.Where(r => businessRoles.Contains(r))
			.Distinct()
			.ToList();

		if (!roles.Any())
		{
			roles.Add("user"); 
		}
		var result = await _notificationService.GetAll(request, userId, roles);

		return Ok(result);
	}

	[Authorize]
	[HttpPut("mark-as-read/{notificationId:guid}")]
	public async Task<IActionResult> MarkAsRead(Guid notificationId)
	{
		await _notificationService.MarkAsRead(notificationId);

		return NoContent();
	}
}
