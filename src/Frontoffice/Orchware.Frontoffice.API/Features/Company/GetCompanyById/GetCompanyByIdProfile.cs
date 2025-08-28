using AutoMapper;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Company.GetCompanyById;

public class GetCompanyByIdProfile : Profile
{
	public GetCompanyByIdProfile()
	{
		CreateMap<Domain.Company, GetCompanyByIdResponse>()
			.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.PhoneConverter()));
	}
}
