namespace Orchware.Frontoffice.API.Domain;

public abstract class Base
{
	public int Id { get; set; }
}

public class BaseNamed : Base
{
	public string Name { get; set; } = string.Empty;
}

public class BaseAuditableEntity
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}

public class AuditableEntity : Base
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}

public class NamedAuditableEntity : BaseNamed
{
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}
