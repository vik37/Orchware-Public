using Microsoft.AspNetCore.SignalR;
using Orchware.Notification.API.Settings;
using System.Diagnostics;
using System.Security.Claims;

namespace Orchware.Notification.API.Hubs;

public class NotificationHub : Hub
{
	public override async Task OnConnectedAsync()
	{
		SignalRTelemetry.ActiveConnections.Add(1);

		using var activity =
			SignalRTelemetry.ActivitySource.StartActivity("signalr.connect");

		activity?.SetTag("connection.id", Context.ConnectionId);
		activity?.SetTag("user.id", Context.UserIdentifier);

		if (Context.User?.Identity?.IsAuthenticated == false)
		{
			activity?.SetTag("authenticated", false);
			Console.WriteLine("[SignalR] WARNING: User is NOT authenticated!");
		}
		else
		{
			activity?.SetTag("authenticated", true);
		}

		var roles = Context.User?.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
										.Select(c => c.Value)
										.ToList();

		if (roles != null && roles.Any())
		{
			activity?.SetTag("roles.count", roles.Count);
			foreach (var role in roles)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, role);
				Console.WriteLine($"[SignalR] Success! Added {Context.ConnectionId} to group: {role}");
			}
		}

		var userId = Context.UserIdentifier;
		if (!string.IsNullOrEmpty(userId))
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, userId);
			activity?.SetTag("user.group", userId);
		}

		await base.OnConnectedAsync();
	}

	public async Task SendMessage(string user, string message)
	{
		await Clients.All.SendAsync("ReceiveNotification", user, message);
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		SignalRTelemetry.ActiveConnections.Add(-1);

		using var activity =
			SignalRTelemetry.ActivitySource.StartActivity("signalr.disconnect");

		activity?.SetTag("connection.id", Context.ConnectionId);
		activity?.SetTag("user.id", Context.UserIdentifier);

		if (exception != null)
		{
			activity?.AddException(exception);
			activity?.SetStatus(ActivityStatusCode.Error);
		}

		await base.OnDisconnectedAsync(exception);
	}
}
