namespace FileStorage.FileFormatServices;

public sealed class ExcelFormatFileService<T> : IFileFormat<T>, IDisposable
	where T : class, new()
{
	private bool _disposed;

	public async Task<IEnumerable<T>> ReadFromFile(string filePath, FileFormatOptions? fileFormatOptions = null)
	{
		// Implement Excel file reading logic here
		return await Task.FromResult(new List<T>());
	}
	public void Dispose()
	{
		if (!_disposed)
			_disposed = true;
	}
}
