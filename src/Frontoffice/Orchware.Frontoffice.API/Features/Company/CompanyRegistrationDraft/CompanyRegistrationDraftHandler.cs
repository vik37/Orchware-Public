using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistrationDraft;

public class CompanyRegistrationDraftHandler : IRequestHandler<CompanyRegistrationDraftCommand, Unit>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;

	public CompanyRegistrationDraftHandler(IRedisDistributedCacheService redisDistributedCacheService,
		IUserContextService userContextService)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
	}

	public async Task<Unit> Handle(CompanyRegistrationDraftCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return Unit.Value;
		if (_userContextService is null || !_userContextService.Id.HasValue)
			return Unit.Value;

		string key = $"company_user_draft:{_userContextService.Id}";
		await _redisDistributedCacheService.SetData<Infrastructure.Cache.Models.CompanyRegistrationDraft>(key, MapRequestToCompanyRegistrationDraft(request), TimeSpan.FromDays(7));

		return Unit.Value;
	}

	private Infrastructure.Cache.Models.CompanyRegistrationDraft MapRequestToCompanyRegistrationDraft(CompanyRegistrationDraftCommand request)
	{
		return new Infrastructure.Cache.Models.CompanyRegistrationDraft
		{
			CompanyName = request.CompanyName,
			CompanyAddress = request.CompanyAddress,
			CompanyCity = request.CompanyCity,
			CompanyEmail = request.CompanyEmail,
			CompanyLocation = request.CompanyLocation,
			CompanyPhoneNumber = request.CompanyPhoneNumber,
			JobTitle = request.JobTitle
		};
	}
}
