namespace Orchware.Backoffice.Application.Features.Shared.CustomExceptions;

public class BadRequestException : Exception
{
	public IDictionary<string, string[]>? ValidationErrors { get; }

	public BadRequestException(string message)
		: base(message){}

	public BadRequestException(string message, IDictionary<string, string[]> validationErrors)
		: base(message)
	{
		ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
	}
}
