using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Company.AssignUserToCompany;

public class AssignUserToCompanyHandler : IRequestHandler<AssignUserToCompanyCommand, string>
{
	private readonly OrchwareDbContext _context;
	private readonly IUserContextService _userContextService;

	public AssignUserToCompanyHandler(OrchwareDbContext context, IUserContextService userContextService)
	{
		_context = context;
		_userContextService = userContextService;
	}

	public async Task<string> Handle(AssignUserToCompanyCommand request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var user = await _context.User.FindAsync(new object[] { _userContextService.Id }, cancellationToken);
		if (user is null)
		{
			var newUser = new User
			{
				Id = _userContextService.Id.Value,
				Name = $"{_userContextService.Firstname} {_userContextService.Lastname}",
				PersonalEmail = _userContextService.Email!,
				JobTitle = request.JobTitle,
				CompanyId = request.CompanyId
			};

			await _context.User.AddAsync(newUser, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);

			return $"{newUser.Name} successfully assigned.";
		}

		user.CompanyId = request.CompanyId;
		await _context.SaveChangesAsync(cancellationToken);

		return $"{user.Name} successfully reassigned.";
	}
}
