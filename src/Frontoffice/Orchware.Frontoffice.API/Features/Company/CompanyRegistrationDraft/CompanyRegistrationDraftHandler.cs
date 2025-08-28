using AutoMapper;
using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistrationDraft;

public class CompanyRegistrationDraftHandler : IRequestHandler<CompanyRegistrationDraftCommand, Unit>
{
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;
	private readonly IUserContextService _userContextService;
	private readonly IMapper _mapper;

	public CompanyRegistrationDraftHandler(IRedisDistributedCacheService redisDistributedCacheService,
		IUserContextService userContextService, IMapper mapper)
	{
		_redisDistributedCacheService = redisDistributedCacheService;
		_userContextService = userContextService;
		_mapper = mapper;
	}

	public async Task<Unit> Handle(CompanyRegistrationDraftCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return Unit.Value;
		if (_userContextService is null || !_userContextService.Id.HasValue)
			return Unit.Value;

		string key = $"company_user_draft:{_userContextService.Id}";
		await _redisDistributedCacheService.SetData<CompanyRegistrationDraft>(key, _mapper.Map<CompanyRegistrationDraft>(request), TimeSpan.FromDays(7));

		return Unit.Value;
	}
}
