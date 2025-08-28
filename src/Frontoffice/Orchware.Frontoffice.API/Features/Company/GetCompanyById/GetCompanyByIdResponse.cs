namespace Orchware.Frontoffice.API.Features.Company.GetCompanyById;

public class GetCompanyByIdResponse
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
}
