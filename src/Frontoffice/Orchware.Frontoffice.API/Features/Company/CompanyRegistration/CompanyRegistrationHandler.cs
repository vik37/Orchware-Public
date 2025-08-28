using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Company.CompanyRegistration;

public class CompanyRegistrationHandler : IRequestHandler<CompanyRegistrationCommand, int>
{
	private readonly OrchwareDbContext _context;
	private readonly IUserContextService _userContextService;
	private readonly IRedisDistributedCacheService _redisDistributedCacheService;

	public CompanyRegistrationHandler(OrchwareDbContext context, IUserContextService userContextService, 
		IRedisDistributedCacheService redisDistributedCacheService)
	{
		_context = context;
		_userContextService = userContextService;
		_redisDistributedCacheService = redisDistributedCacheService;
	}

	public async Task<int> Handle(CompanyRegistrationCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
			return 0;

		if (_userContextService is null || !_userContextService.Id.HasValue)
			throw new ForbiddenException("You don't have permission to register a company");

		string key = $"company_user_draft:{_userContextService.Id}";
		await _redisDistributedCacheService.RemoveData(key);

		var existingCompany = await _context.Company
											 .FirstOrDefaultAsync(x => x.Name == request.CompanyName &&
																	   x.City == request.CompanyCity &&
																	   x.Location == request.CompanyLocation, cancellationToken);

		if (existingCompany is not null)
			throw new EntityAlreadyExistsException("Company", existingCompany.Name, existingCompany.Id, detail: $"Job Title: {request.JobTitle}");

		var existingUser = await _context.User.FindAsync(_userContextService.Id.Value);

		if (existingUser is not null)
		{
			_context.User.Remove(existingUser);
			await _context.SaveChangesAsync(cancellationToken);
		}

		var company = new Domain.Company
		{
			Name = request.CompanyName,
			City = request.CompanyCity,
			Location = request.CompanyLocation,
			Email = request.CompanyEmail,
			Address = request.CompanyAddress,
			Phone = request.CompanyPhoneNumber,
			Budget = (decimal)new Random().NextDouble() * 10000m
		};

		await _context.Company.AddAsync(company, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		var newUser = new User
		{
			Id = _userContextService.Id.Value,
			Name = $"{_userContextService.Firstname} {_userContextService.Lastname}",
			PersonalEmail = _userContextService.Email!,
			JobTitle = request.JobTitle,
			CompanyId = company.Id
		};

		await _context.User.AddAsync(newUser, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return company.Id;
	}
}
