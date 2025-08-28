namespace Utilities.Converters;

public static class PhoneConverterExtensions
{
	public static string PhoneConverter(this string phoneNumber)
	{
		var cleanedNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

		if (cleanedNumber.Length < 8 || cleanedNumber.Length > 10)
			throw new ArgumentException("Invalid phone number. It must be between 8 and 10 digits.");

		var remainingDigits = cleanedNumber.Length - 2;

		var dashPosition = remainingDigits / 2;

		return $"(+389) {cleanedNumber.Substring(0, 2)}/{cleanedNumber.Substring(2, dashPosition)}-{cleanedNumber.Substring(2 + dashPosition, remainingDigits - dashPosition)}";
	}
}
