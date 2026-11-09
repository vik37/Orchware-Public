using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;

namespace Orchware.Backoffice.Infrastructure.Files.FileTypes;

public class LocalFileStorageService : IFileStorageService
{
	Task<Stream> IFileStorageService.DownloadAsync(string fileName)
	{
		throw new NotImplementedException();
	}
}
