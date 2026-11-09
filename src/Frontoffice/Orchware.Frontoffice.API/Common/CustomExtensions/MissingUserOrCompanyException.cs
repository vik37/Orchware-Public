namespace Orchware.Frontoffice.API.Common.CustomExtensions;

public class MissingUserOrCompanyException : Exception
{
	public MissingUserOrCompanyException(Guid userId)
		: base($"User {userId} does not exist or does not have an associated Company.")
	{}
}
