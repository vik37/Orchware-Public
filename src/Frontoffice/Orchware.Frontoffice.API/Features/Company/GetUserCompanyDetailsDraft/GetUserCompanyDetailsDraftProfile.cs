using AutoMapper;
using Orchware.Frontoffice.API.Features;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetailsDraft;

public class GetUserCompanyDetailsDraftProfile : Profile
{
	public GetUserCompanyDetailsDraftProfile()
	{
		CreateMap<Company.CompanyRegistrationDraft.CompanyRegistrationDraft, GetUserCompanyDetailsDraftResponse>();
	}
}
