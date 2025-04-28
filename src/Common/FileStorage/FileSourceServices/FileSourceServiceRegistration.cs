using Microsoft.Extensions.DependencyInjection;

namespace FileStorage.FileSourceServices;

internal static class FileSourceServiceRegistration
{
	internal static IServiceCollection AddFileSourceServices(this IServiceCollection services)
	{
		services.AddTransient(typeof(LocalFileStorage<,>));
		services.AddTransient(typeof(AzureFileStorage<,>));

		return services;
	}
}
