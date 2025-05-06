using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Utilities.Converters;

public static class EnumExtensions
{
	public static string GetDisplayNameToString(this Enum enumValue)
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
		var attribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
		return attribute?.Name ?? enumValue.ToString();
	}
}
