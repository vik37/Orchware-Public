using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public class Company : ValueObject
{
	public string Buyer { get; init; } = string.Empty;
	public string JobTitle { get; init; } = string.Empty;
	public string PersonalEmail { get; init; } = string.Empty;
	public string CompanyEmail { get; init; } = string.Empty;
	public string CompanyAddress { get; init; } = string.Empty;

	protected override IEnumerable<object> GetValues()
	{
		yield return Buyer;
		yield return JobTitle;
		yield return PersonalEmail;
		yield return CompanyEmail;
		yield return CompanyAddress;
	}
}
