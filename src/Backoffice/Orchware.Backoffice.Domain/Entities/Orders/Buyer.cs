using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public class Buyer : ValueObject
{
	public string Name { get; private set; } = string.Empty;
	public string JobTitle { get; private set; } = string.Empty;
	public string PersonalEmail { get; private set; } = string.Empty;
	public string CompanyName { get; private set; } = string.Empty;
	public string CompanyEmail { get; private set; } = string.Empty;
	public string CompanyAddress { get; private set; } = string.Empty;
	public string CompanyCity { get; private set; } = string.Empty;
	public string CompanyLocation { get; private set; } = string.Empty;
	public string CompanyPhoneNumber { get; private set; } = string.Empty;
	public DateTime CompanyRegisteredDate { get; private set; }

	public Buyer(){}

	public Buyer(string name, string jobTitle, string personalEmail, string companyName, string companyEmail,
		string companyAddress, string companyCity, string companyLocation, string companyPhoneNumber, DateTime companyRegisteredDate)
	{
		Name = name; 
		JobTitle = jobTitle;
		PersonalEmail = personalEmail;
		CompanyName = companyName;
		CompanyEmail = companyEmail;
		CompanyAddress = companyAddress;
		CompanyCity = companyCity;
		CompanyLocation = companyLocation;
		CompanyPhoneNumber = companyPhoneNumber;
		CompanyRegisteredDate = companyRegisteredDate;
	}

	protected override IEnumerable<object> GetValues()
	{
		yield return Name;
		yield return JobTitle;
		yield return PersonalEmail;
		yield return CompanyName;
		yield return CompanyEmail;
		yield return CompanyAddress;
		yield return CompanyCity;
		yield return CompanyLocation;
		yield return CompanyPhoneNumber;
		yield return CompanyRegisteredDate;
	}
}
