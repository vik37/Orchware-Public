using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace FileStorage.FileFormatServices;

public sealed class CSVFormatFileService<T> : IFileFormat<T>, IDisposable
	where T : class, new()
{
	private bool _disposed;

	public async Task<IEnumerable<T>> ReadFromFile(string filePath, FileFormatOptions? fileFormatOptions = null)
	{
		using var reader = new StreamReader(filePath);

		using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = true,
			HeaderValidated = null,
			MissingFieldFound = null,
			Delimiter = fileFormatOptions?.Delimiter ?? ","
		});

		var records = csv.GetRecords<T>().ToList();
		return await Task.FromResult(records);
	}

	public void Dispose()
	{
		if(!_disposed)
			_disposed = true;
	}
}