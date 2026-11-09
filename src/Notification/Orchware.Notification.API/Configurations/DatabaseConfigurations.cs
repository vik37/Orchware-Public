using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Infrastructure.Persistence;
using System.Reflection;

namespace Orchware.Notification.API.Configurations;

public static class DatabaseConfigurations
{
	public static IServiceCollection AddDatabaseRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<NotificationDbContext>(opt =>
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

			opt.ConfigureWarnings(warnings =>
				warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.ColumnOrderIgnoredWarning));
		});

		services.AddHealthChecks()
				.AddDbContextCheck<NotificationDbContext>(name: "sql-check", tags: new[] { "ready" });

		return services;
	}
}
