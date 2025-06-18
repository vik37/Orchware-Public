using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orchware.Backoffice.Application.Features.Shared.Options;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;
using System.Reflection;

namespace Orchware.Backoffice.Application;

public static class ApplicationServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddAutoMapper(Assembly.GetExecutingAssembly());
		services.AddMediatR(conf => conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

		services.Configure<DatabaseConnectionOption>(opt =>
		{
			opt.MSSQLDbConnection = configuration.GetConnectionString("MSSQLDbConnection")
				?? throw new ArgumentNullException(nameof(opt.MSSQLDbConnection), "Invalid database connection string.");
		});

		services.AddScoped<ISqlQueryBuilder, SqlQueryBuilder>();

		return services;
	}
}