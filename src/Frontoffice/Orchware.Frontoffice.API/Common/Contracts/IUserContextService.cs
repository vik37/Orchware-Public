namespace Orchware.Frontoffice.API.Common.Contracts;

public interface IUserContextService
{
	public Guid? Id { get; }
	public string? UserName { get; }
	public string? Email { get; }
	public string? Firstname { get; }
	public string? Lastname { get; }
}
