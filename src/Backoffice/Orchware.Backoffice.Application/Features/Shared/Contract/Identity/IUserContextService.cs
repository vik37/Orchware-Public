namespace Orchware.Backoffice.Application.Features.Shared.Contract.Identity;

public interface IUserContextService
{
	public string? Id { get; }
	public string? UserName { get; }
	public string? Email { get; }
	public string? Firstname { get; }
	public string? Lastname { get; }
	public string? Role { get; }
}
