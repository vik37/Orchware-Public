using AutoMapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetailsDraft;

public class GetUserCompanyDetailsDraftHandler : IRequestHandler<GetUserCompanyDetailsDraftQuery, GetUserCompanyDetailsDraftResponse>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;
	private readonly IMapper _mapper;

	public GetUserCompanyDetailsDraftHandler(IRedisDistributedCacheService redisDistributedCacheService, 
		IUserContextService userContextService, IMapper mapper)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
		_mapper = mapper;
	}

	public async Task<GetUserCompanyDetailsDraftResponse> Handle(GetUserCompanyDetailsDraftQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetUserCompanyDetailsDraftResponse();
		if (_userContextService is null || !_userContextService.Id.HasValue)
			return new GetUserCompanyDetailsDraftResponse();

		string key = $"company_user_draft:{_userContextService.Id}";

		var companyRegistrationDraft = await _redisDistributedCacheService.GetData<Company.CompanyRegistrationDraft.CompanyRegistrationDraft>(key);

		if (companyRegistrationDraft == null)
			return new GetUserCompanyDetailsDraftResponse();

		return _mapper.Map<GetUserCompanyDetailsDraftResponse>(companyRegistrationDraft);
	}
}
