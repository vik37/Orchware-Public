using FileStorage.FileFormatServices;

namespace FileStorage.FileSourceServices;

public interface IFileService<T, U> 
	where T : class, new()
	where U : IFileFormat<T>
{
	Task<IEnumerable<T>> ReadFromFile(string filePath, FileFormatOptions? fileFormatOptions = null);
	Task DeleteFile(string filePath);
}
