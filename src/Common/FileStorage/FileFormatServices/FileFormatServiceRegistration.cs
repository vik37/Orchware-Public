using Microsoft.Extensions.DependencyInjection;

namespace FileStorage.FileFormatServices;

internal static class FileFormatServiceRegistration
{
	internal static IServiceCollection AddFileFormatServices(this IServiceCollection services)
	{
		services.AddTransient(typeof(CSVFormatFileService<>));
		services.AddTransient(typeof(ExcelFormatFileService<>));

		return services;
	}
}
