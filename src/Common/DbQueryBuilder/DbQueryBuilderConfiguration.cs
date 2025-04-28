using Microsoft.Extensions.DependencyInjection;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace DbQueryBuilder;

public static class DbQueryBuilderConfiguration
{
	public static IServiceCollection AddDbQueryBuilder(this IServiceCollection services)
	{
		services.AddSingleton<ISqlQueryBuilder, SqlQueryBuilder>();
		return services;
	}
}
