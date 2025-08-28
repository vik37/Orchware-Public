namespace Orchware.Frontoffice.API.Common.CustomExceptions;

public class ForbiddenException : Exception
{
	public ForbiddenException(string message): base(message){}
}
