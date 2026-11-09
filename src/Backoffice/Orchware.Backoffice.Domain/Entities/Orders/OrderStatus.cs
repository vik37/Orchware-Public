using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public class OrderStatus : ValueObject
{
	public string Value { get; }
	public string Color { get; }

	private OrderStatus(string value, string color)
	{
		Value = value;
		Color = color;
	}

	protected override IEnumerable<object> GetValues()
	{
		yield return Value;
		yield return Color;
	}

	public static OrderStatus Requested => new OrderStatus("Requested", "#bed973");
	public static OrderStatus Confirmed => new OrderStatus("Confirmed", "#fa9052");
	public static OrderStatus InPreparation => new OrderStatus("InPreparation", "#18f2e7");
	public static OrderStatus AwaitingDispatch => new OrderStatus("AwaitingDispatch", "#d920fa");
	public static OrderStatus Approved => new OrderStatus("Approved", "#57f75c");
	public static OrderStatus Cancelled => new OrderStatus("Cancelled", "#f00202");

	public static IEnumerable<OrderStatus> GetAllStatuses() => new[]
	{
		Requested, Confirmed, InPreparation, AwaitingDispatch, Approved, Cancelled
	};

	private static readonly Dictionary<OrderStatus, List<OrderStatus>> _transitions = new()
	{
		{ OrderStatus.Requested, new() { OrderStatus.Confirmed, OrderStatus.Cancelled } },
		{ OrderStatus.Confirmed, new() { OrderStatus.InPreparation, OrderStatus.Cancelled } },
		{ OrderStatus.InPreparation, new() { OrderStatus.AwaitingDispatch, OrderStatus.Cancelled } },
		{ OrderStatus.AwaitingDispatch, new() { OrderStatus.Approved, OrderStatus.Cancelled } },
		{ OrderStatus.Approved, new() { } },
        { OrderStatus.Cancelled, new() { } }
    };

	public static bool CanTransition(OrderStatus from, OrderStatus to)
		=> _transitions.TryGetValue(from, out var nextStates) && nextStates.Contains(to);
}
