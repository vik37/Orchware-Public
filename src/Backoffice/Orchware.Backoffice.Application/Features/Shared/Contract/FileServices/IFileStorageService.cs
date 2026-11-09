namespace Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;

public interface IFileStorageService
{
	Task<Stream> DownloadAsync(string fileName);
}
