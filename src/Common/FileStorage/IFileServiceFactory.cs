using FileStorage.Enums;
using FileStorage.FileFormatServices;
using FileStorage.FileSourceServices;

namespace FileStorage;

public interface IFileServiceFactory
{
	IFileService<T,U> Create<T,U>(FileSource fileSource, FileFormat fileFormat)
		where T : class, new()
		where U : IFileFormat<T>;
}
