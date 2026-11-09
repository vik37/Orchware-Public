using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;

namespace Orchware.Frontoffice.API.Features.Company.GetUserCompanyDetailsDraft;

public class GetUserCompanyDetailsDraftHandler : IRequestHandler<GetUserCompanyDetailsDraftQuery, GetUserCompanyDetailsDraftResponse>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;

	public GetUserCompanyDetailsDraftHandler(IRedisDistributedCacheService redisDistributedCacheService, 
		IUserContextService userContextService)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
	}

	public async Task<GetUserCompanyDetailsDraftResponse> Handle(GetUserCompanyDetailsDraftQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return new GetUserCompanyDetailsDraftResponse();
		if (_userContextService is null || !_userContextService.Id.HasValue)
			return new GetUserCompanyDetailsDraftResponse();

		string key = $"company_user_draft:{_userContextService.Id}";

		var companyRegistrationDraft = await _redisDistributedCacheService.GetData<Infrastructure.Cache.Models.CompanyRegistrationDraft>(key);

		if (companyRegistrationDraft == null)
			return new GetUserCompanyDetailsDraftResponse();

		return MapCompanyUserDraftToGetUserCompanyDetailsDraftResponse(companyRegistrationDraft);
	}

	private GetUserCompanyDetailsDraftResponse MapCompanyUserDraftToGetUserCompanyDetailsDraftResponse(Infrastructure.Cache.Models.CompanyRegistrationDraft company)
		=> new GetUserCompanyDetailsDraftResponse
		{
			CompanyAddress = company.CompanyAddress,
			CompanyCity = company.CompanyCity,
			CompanyEmail = company.CompanyEmail,
			CompanyLocation = company.CompanyLocation,
			CompanyName = company.CompanyName,
			CompanyPhoneNumber = company.CompanyPhoneNumber,
			JobTitle = company.JobTitle
		};
}
