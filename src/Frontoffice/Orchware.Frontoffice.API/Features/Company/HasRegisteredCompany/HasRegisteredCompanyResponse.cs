namespace Orchware.Frontoffice.API.Features.Company.HasRegisteredCompany;

public class HasRegisteredCompanyResponse
{
	public int? CompanyId { get; set; }
	public bool UserHasRegisteredCompany { get; set; } = false;
}
