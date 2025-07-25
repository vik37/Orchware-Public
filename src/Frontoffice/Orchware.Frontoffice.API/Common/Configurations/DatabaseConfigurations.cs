using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Infrastructure.Persistence;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Dapper;
using System.Reflection;

namespace Orchware.Frontoffice.API.Common.Configurations;

public static class DatabaseConfigurations
{
	public static IServiceCollection AddDatabaseRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<OrchwareDbContext>(opt =>
		{
			var connectionString = configuration.GetConnectionString("MSSQLDbConnection")
				?? throw new InvalidOperationException("Connection string not found!");

			opt.UseSqlServer(connectionString, sqlOptions =>
			{
				sqlOptions.EnableRetryOnFailure(
					maxRetryCount: 5,
					maxRetryDelay: TimeSpan.FromSeconds(5),
					errorNumbersToAdd: null
				);

				sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
			});
		});

		services.AddHealthChecks().AddDbContextCheck<OrchwareDbContext>();

		services.AddScoped<DapperContext>();

		return services;
	}
}
