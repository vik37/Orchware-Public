namespace Orchware.Backoffice.Application.Features.Shared.CustomExceptions;

public class NotFoundException : Exception
{
	public NotFoundException(string message) : base(message){}
}
