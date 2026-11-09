using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices.Enum;

namespace Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;

public interface IFileStorageFactory
{
	IFileStorageService Initialize(FileType types);
}
