using FileStorage.FileFormatServices;

namespace FileStorage.FileSourceServices;

public sealed class AzureFileStorage<T, U> : IFileService<T, U>, IDisposable
	where T : class, new()
	where U : IFileFormat<T>
{
	private bool _disposed;
	private U _fileFormat;

	public AzureFileStorage(U fileFormat)
	{
		_fileFormat = fileFormat;
	}

	public Task<IEnumerable<T>> ReadFromFile(string filePath, FileFormatOptions? fileFormatOptions = null)
	{
		throw new NotImplementedException();
	}

	public Task DeleteFile(string filePath)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
		}
	}
}
