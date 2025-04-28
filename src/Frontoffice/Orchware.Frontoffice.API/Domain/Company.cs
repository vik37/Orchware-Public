namespace Orchware.Frontoffice.API.Domain;

public class Company : NamedAuditableEntity
{
	public string UserId { get; set; } = string.Empty;
	public string Buyer { get; set; } = string.Empty;
	public string JobTitle { get; set; } = string.Empty;
	public string PersonalEmail { get; set; } = string.Empty;
	public string CompanyEmail { get; set; } = string.Empty;

	private decimal _budget;

	public decimal Budget 
	{ 
		get => _budget == 0 ? GetRandomBudhet() : _budget; 
		set => _budget = value; 
	}

	private static decimal GetRandomBudhet()
	{
		var random = new Random();
		return (decimal)random.NextDouble() * 10000m;
	}

	public ICollection<Order>? Orders { get; set; }
}
