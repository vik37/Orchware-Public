namespace Utilities.Converters;

public static class CustomNumberGenerator
{
	public static string FiveDigitGenerator()
	{
		Random random = new Random();
		int randomNumber = random.Next(0, 100000);
	    return randomNumber.ToString("D5");
	}
}
