using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public class VisibilityLevel : ValueObject
{
	public string Value { get; }

	private VisibilityLevel(string value)
	{
		Value = value;
	}

	protected override IEnumerable<object?> GetValues()
	{
		yield return Value;
	}

	public static VisibilityLevel Manager => new VisibilityLevel("manager");
	public static VisibilityLevel Warehouseman => new VisibilityLevel("warehouseman");

	public static VisibilityLevel FromString(string value) => value switch
	{
		"manager" => Manager,
		"warehouseman" => Warehouseman,
		_ => throw new ArgumentException($"Invalid processing level: {value}")
	};
}
