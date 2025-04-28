namespace FileStorage.FileFormatServices;

public interface IFileFormat<T> where T : class, new()
{
	Task<IEnumerable<T>> ReadFromFile(string filePath, FileFormatOptions? fileFormatOptions = null);
}
