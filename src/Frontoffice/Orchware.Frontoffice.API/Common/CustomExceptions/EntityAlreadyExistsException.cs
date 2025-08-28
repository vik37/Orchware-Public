namespace Orchware.Frontoffice.API.Common.CustomExceptions;

public class EntityAlreadyExistsException : Exception
{
	public string EntityName { get; }
	public string Identifier { get; }
	public int? ExistingEntityId { get; }
	public string? Detail { get; }

	public EntityAlreadyExistsException(string entityName, string identifier,  string message, 
											int? existingEntityId = null, string? detail = null)
		: base(message)
	{
		EntityName = entityName;
		Identifier = identifier;
		ExistingEntityId = existingEntityId;
		Detail = detail;
	}

	public EntityAlreadyExistsException(string entityName, string identifier, int? existingEntityId = null, string? detail = null)
		: base($"{entityName} with identifier '{identifier}' already exists.")
	{
		EntityName = entityName;
		Identifier = identifier;
		ExistingEntityId = existingEntityId;
		Detail = detail;
	}
}
