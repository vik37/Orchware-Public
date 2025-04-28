using FileStorage.FileFormatServices;

namespace FileStorage.FileSourceServices;

public sealed class LocalFileStorage<T, U> : IFileService<T, U>, IDisposable
	where T : class, new()
	where U : IFileFormat<T>
{
	private bool _disposed;
	private U _fileFormat;

	public LocalFileStorage(U fileFormat)
	{
		_fileFormat = fileFormat;
	}

	public async Task<IEnumerable<T>> ReadFromFile(string filePath, FileFormatOptions? fileFormatOptions = null)
	{
		return await _fileFormat.ReadFromFile(filePath, fileFormatOptions);
	}

	public async Task DeleteFile(string filePath)
	{
		if(File.Exists(filePath))
		{
			File.Delete(filePath);
		}

		await Task.CompletedTask;
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
		}
	}
}
