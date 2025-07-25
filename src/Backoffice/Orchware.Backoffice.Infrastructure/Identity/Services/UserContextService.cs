using Microsoft.AspNetCore.Http;
using Orchware.Backoffice.Application.Features.Shared.Contract.Identity;
using System.Security.Claims;

namespace Orchware.Backoffice.Infrastructure.Identity.Services;

public class UserContextService : IUserContextService
{
	private readonly IHttpContextAccessor _contextAccessor;

	public UserContextService(IHttpContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	public string? Id => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	public string? UserName => _contextAccessor.HttpContext?.User?.Identity?.Name;

	public string? Email => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

	public string? Firstname => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

	public string? Lastname => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value;

	public string? Role => _contextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role)
																.Select(r => r.Value)
																.FirstOrDefault(x => x.Contains("manager") 
																				|| x.Contains("warehouseman"));
}
