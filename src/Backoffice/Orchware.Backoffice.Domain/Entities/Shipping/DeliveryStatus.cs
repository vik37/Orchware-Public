using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Shipping;

public class DeliveryStatus : ValueObject
{
	public string Value { get; }
	public string Color { get; }

	private DeliveryStatus(string value, string color)
	{
		Value = value;
		Color = color;
	}

	protected override IEnumerable<object> GetValues()
	{
		yield return Value;
		yield return Color;
	}

	public static DeliveryStatus InTransit => new DeliveryStatus("In-Transit", "#185996");
	public static DeliveryStatus Delivered => new DeliveryStatus("Delivered", "#35910d");
	public static DeliveryStatus Failed => new DeliveryStatus("Filed", "#a61f1f");

	public static IEnumerable<DeliveryStatus> GetAllStatuses() => new[]
	{
		InTransit, Delivered, Failed
	};
}
