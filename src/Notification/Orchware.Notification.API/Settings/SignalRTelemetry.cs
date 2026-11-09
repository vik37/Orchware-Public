using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Orchware.Notification.API.Settings;

public static class SignalRTelemetry
{
	public const string ActivitySourceName = "Orchware.SignalR";
	public const string MeterName = "Orchware.SignalR";

	public static readonly ActivitySource ActivitySource =
		new(ActivitySourceName);

	public static readonly Meter Meter =
		new(MeterName);

	public static readonly UpDownCounter<long> ActiveConnections =
		Meter.CreateUpDownCounter<long>(
			"signalr.active_connections",
			unit: "connections",
			description: "Currently active SignalR connections");

	public static readonly Counter<long> MessagesSent =
		Meter.CreateCounter<long>(
			"signalr.messages.sent",
			unit: "messages",
			description: "Total SignalR messages sent");

	public static readonly Counter<long> MessagesReceived =
		Meter.CreateCounter<long>(
			"signalr.messages.received",
			unit: "messages",
			description: "Total SignalR messages received");
}
