using MediatR;
using Orchware.Frontoffice.API.Common.Contracts;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Infrastructure.Persistence;

namespace Orchware.Frontoffice.API.Features.Company.EditUserJobTitle;

public class EditUserCompanyJobTitleHandler : IRequestHandler<EditUserCompanyJobTitleCommand, Unit>
{
	private readonly OrchwareDbContext _context;
	private readonly IUserContextService _userContextService;

	public EditUserCompanyJobTitleHandler(OrchwareDbContext context, IUserContextService userContextService)
	{
		_context = context;
		_userContextService = userContextService;
	}

	public async Task<Unit> Handle(EditUserCompanyJobTitleCommand request, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		if (_userContextService?.Id is null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		var user = await _context.User.FindAsync(new object[] { _userContextService.Id }, cancellationToken);
		if (user is null)
			throw new NotFoundException("User not found.");

		if (user.CompanyId != user.CompanyId)
			throw new ForbiddenException("You can only update user roles within your own company.");

		user.JobTitle = request.JobTitle;

		await _context.SaveChangesAsync();
		return Unit.Value;
	}
}
