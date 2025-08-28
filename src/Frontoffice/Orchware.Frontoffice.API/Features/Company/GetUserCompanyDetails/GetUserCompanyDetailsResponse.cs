using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetails;

public class GetUserCompanyDetailsResponse
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string PersonalEmail { get; set; } = string.Empty;
	public string JobTitle { get; set; } = string.Empty;
	public int? CompanyId { get; set; }
	public string CompanyName { get; set; } = string.Empty;
	public string CompanyEmail { get; set; } = string.Empty;
	public string CompanyCity { get; set; } = string.Empty;
	public string CompanyLocation { get; set; } = string.Empty;
	public string CompanyAddress { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }

	private string _phone = string.Empty;
	public string CompanyPhone
	{
		get
		{
			return _phone.PhoneConverter();
		}
		set
		{
			_phone = value;
		}
	}
}
