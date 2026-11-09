using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Utilities.Converters;

public static class CustomEnumExtensions
{
	public static string GetDisplayNameToString(this Enum enumValue)
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
		var attribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
		return attribute?.Name ?? enumValue.ToString();
	}

	public static TEnum GetEnumFromDisplayName<TEnum>(this string displayName) where TEnum : struct, IConvertible
	{
		if (!typeof(TEnum).IsEnum)
		{
			throw new ArgumentException("T must be an enumerated type");
		}

		#nullable disable
		foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
		{
			var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
			if (displayAttribute != null && displayAttribute.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase))
			{
				return (TEnum)field.GetValue(null);
			}
		}
		#nullable enable

		if (Enum.TryParse(displayName, true, out TEnum result))
		{
			return result;
		}

		return default;
	}
}
