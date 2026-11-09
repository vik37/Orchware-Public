using Microsoft.Extensions.DependencyInjection;
using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices;
using Orchware.Backoffice.Application.Features.Shared.Contract.FileServices.Enum;

namespace Orchware.Backoffice.Infrastructure.Files;

public class FileStorageFactory : IFileStorageFactory
{
	private readonly IServiceProvider _serviceProvider;

	public FileStorageFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IFileStorageService Initialize(FileType types)
	{
		return types switch
		{
			FileType.Local => _serviceProvider.GetRequiredKeyedService<IFileStorageService>("local"),
			FileType.Azure => _serviceProvider.GetRequiredKeyedService<IFileStorageService>("azure"),
			_ => throw new ArgumentOutOfRangeException(nameof(types))
		};
	}
}
