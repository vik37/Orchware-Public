using AutoMapper;
using Orchware.Frontoffice.API.Domain;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Features.Products.FilteredProducts;

public class FilteredProductProfile : Profile
{
	public FilteredProductProfile()
	{
		CreateMap<Product, FilteredProductsDto>()
			.ForMember(sourse => sourse.SeasonalFruits, dest => dest.MapFrom(x => x.SeasonalFruits.GetDisplayNameToString()))
			.ForMember(source => source.Units, dest => dest.MapFrom(x => x.Units.GetDisplayNameToString()));
	}
}
