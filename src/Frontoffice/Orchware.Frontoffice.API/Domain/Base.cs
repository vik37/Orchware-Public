namespace Orchware.Frontoffice.API.Domain;

public abstract class BaseInt
{
	public int Id { get; set; }
}

public class BaseNamedInt : BaseInt
{
	public string Name { get; set; } = string.Empty;
}

public class BaseAuditableEntityInt : BaseInt, IAuditable
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}

public class NamedAuditableEntityInt : BaseNamedInt, IAuditable
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}

public abstract class BaseGuid
{
	public Guid Id { get; set; }
}

public class BaseNamedGuid : BaseGuid
{
	public string Name { get; set; } = string.Empty;
}

public class BaseAuditableEntityGuid : BaseGuid, IAuditable
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}

public class NamedAuditableEntityGuid : BaseNamedGuid, IAuditable
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}

public interface IAuditable
{
	DateTime CreatedDate { get; set; }
	DateTime? ModifiedDate { get; set; }
}