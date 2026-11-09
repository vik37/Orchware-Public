namespace Utilities.Converters;

public static class UnitConverter
{
	private static readonly Dictionary<(string From, string To), decimal> _conversionFactors =
		new()
		{
			{ ("t", "kg"), 1000m },
			{ ("kg", "g"), 1000m },
			{ ("t", "g"), 1000000m }
		};

	private static string Normalize(string unit) => unit.ToLower() switch
	{
		"tone" or "ton" or "tones" or "tons" => "t",
		"kilogram" or "kilograms" or "kg" => "kg",
		"gram" or "grams" or "g" => "g",
		_ => unit.ToLower()
	};

	public static decimal Convert(decimal value, string fromUnit, string toUnit)
	{
		string from = Normalize(fromUnit);
		string to = Normalize(toUnit);

		if (from == to)
			return value;

		if (_conversionFactors.TryGetValue((from, to), out decimal factor))
		{
			return value * factor;
		}

		if (_conversionFactors.TryGetValue((to, from), out decimal reverseFactor))
		{
			return value / reverseFactor;
		}

		throw new ArgumentException($"Conversion from {fromUnit} to {toUnit} is not supported.");
	}
}
