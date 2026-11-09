namespace Orchware.Backoffice.Application.Features.Shared.CustomExceptions;

public class ForbiddenException : Exception
{
	public ForbiddenException(string message) : base(message) { }
}
