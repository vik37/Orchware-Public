using FileStorage.FileFormatServices;
using FileStorage.FileSourceServices;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage;

public static class FileServiceConfiguration
{
	public static IServiceCollection AddFileServices(this IServiceCollection services)
	{
		services
			.AddFileFormatServices()
			.AddFileSourceServices();

		services.AddSingleton<IFileServiceFactory,FileServiceFactory>();

		return services;
	}
}