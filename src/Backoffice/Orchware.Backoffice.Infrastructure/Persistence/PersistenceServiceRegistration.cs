using FileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orchware.Backoffice.Infrastructure.Persistence;

public static class PersistenceServiceRegistration
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<OrchwareBackofficeDbContext>(opt =>
		{
			opt.UseSqlServer(configuration.GetConnectionString("MSSQLDbConnection"));
		});

		services.AddFileServices();
		services.AddScoped<OrchwareBackInitializer>();

		return services;
	}
}
