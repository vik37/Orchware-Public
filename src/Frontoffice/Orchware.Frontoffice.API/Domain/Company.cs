namespace Orchware.Frontoffice.API.Domain;

public class Company : NamedAuditableEntityInt
{
	public string Email { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public decimal Budget{ get; set; }

	public ICollection<User> Users { get; set; } = new List<User>();

	public ICollection<Order>? Orders { get; set; }
}
