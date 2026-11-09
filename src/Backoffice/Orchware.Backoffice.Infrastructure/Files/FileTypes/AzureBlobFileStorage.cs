using Azure.Storage.Blobs;
using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;

namespace Orchware.Backoffice.Infrastructure.Files.FileTypes;

public class AzureBlobFileStorage : IFileStorageService
{
	private readonly BlobServiceClient _blobServiceClient;

	public AzureBlobFileStorage(BlobServiceClient blobServiceClient)
	{
		_blobServiceClient = blobServiceClient;
	}

	public async Task<Stream> DownloadAsync(string fileName)
	{
		var uri = new Uri(fileName);
		var containerName = uri.Segments[1].Trim('/');
		var blobName = string.Join("", uri.Segments.Skip(2));

		var blobClient = _blobServiceClient
			.GetBlobContainerClient(containerName)
			.GetBlobClient(blobName);

		if (!await blobClient.ExistsAsync())
			throw new FileNotFoundException($"File not found in the storage: {fileName}");

		var stream = await blobClient.OpenReadAsync();
		return stream;
	}
}
