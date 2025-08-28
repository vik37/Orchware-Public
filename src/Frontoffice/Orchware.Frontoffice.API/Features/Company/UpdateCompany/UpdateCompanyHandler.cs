using MediatR;
using Orchware.Frontoffice.API.Common.Constants;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Company.UpdateCompany;

public class UpdateCompanyHandler : IRequestHandler<UpdateCompanyCommand, Unit>
{
	private readonly OrchwareDbContext _context;
	private readonly IUserContextService _userContextService;

	public UpdateCompanyHandler(OrchwareDbContext context, IUserContextService userContextService)
	{
		_context = context;
		_userContextService = userContextService;
	}

	public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var user = await _context.User.FindAsync(new object[] { _userContextService.Id }, cancellationToken);
		if (user is null)
			throw new NotFoundException("User not found.");

		if (user.JobTitle != UserCompanyDefaultRoles.CompanyOwner && user.JobTitle != UserCompanyDefaultRoles.CompanyAdmin)
		{
			throw new ForbiddenException("You don't have permission to update company details.");
		}

		if (user.CompanyId != request.CompanyId)
		{
			throw new ForbiddenException("You can only update your own company's details.");
		}

		var company = await _context.Company.FindAsync(new object[] { request.CompanyId }, cancellationToken);
		if (company is null)
			throw new NotFoundException("Company does not exist.");

		company.Name = request.CompanyName;
		company.City = request.CompanyCity;
		company.Location = request.CompanyLocation;
		company.Email = request.CompanyEmail;
		company.Address = request.CompanyAddress;
		company.Phone = request.CompanyPhoneNumber;
		company.ModifiedDate = DateTime.UtcNow;

		await _context.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}
