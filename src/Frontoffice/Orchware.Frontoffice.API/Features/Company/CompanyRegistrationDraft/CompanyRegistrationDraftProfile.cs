using AutoMapper;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistrationDraft;

public class CompanyRegistrationDraftProfile : Profile
{
	public CompanyRegistrationDraftProfile()
	{
		CreateMap<CompanyRegistrationDraftCommand, CompanyRegistrationDraft>();
	}
}
