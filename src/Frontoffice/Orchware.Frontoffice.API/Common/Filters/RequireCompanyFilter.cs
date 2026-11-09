using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Common.CustomExceptions;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using System.Security.Claims;

namespace Orchware.Frontoffice.API.Common.Filters;

public class RequireCompanyFilter : IAsyncActionFilter
{
	private readonly OrchwareDbContext _context;

	public RequireCompanyFilter(OrchwareDbContext context)
	{
		_context = context;
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId))
		{
			context.Result = new UnauthorizedObjectResult(new { Message = "User not authenticated." });
			return;
		}

		bool isGuid = Guid.TryParse(userId, out Guid res);
		if (!isGuid)
		{
			context.Result = new UnauthorizedObjectResult(new { Message = "User not authenticated." });
			return;
		}

		var user = await _context.User.FirstOrDefaultAsync(x => x.Id == res);

		if (user == null)
			throw new ForbiddenException("You don't have permission to perform this action.");

		if(!user.CompanyId.HasValue)
			throw new ForbiddenException("You don't have permission to perform this action.");

		await next();
	}
}
