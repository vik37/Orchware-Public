namespace Orchware.Frontoffice.API.Domain;

public class User : BaseNamedGuid
{
	public string JobTitle { get; set; } = string.Empty;
	public string PersonalEmail { get; set; } = string.Empty;

	public int? CompanyId { get; set; }
	public Company? Company { get; set; }

	public ICollection<Order> Orders { get; set; } = new List<Order>();
}
