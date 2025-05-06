using DbQueryBuilder.SqlPremmissionProvider;
using Microsoft.Extensions.DependencyInjection;
using Orchware.Frontoffice.API.Common.SQLScriptBuilder;

namespace DbQueryBuilder;

public static class DbQueryBuilderConfiguration
{
	public static IServiceCollection AddDbQueryBuilder<TPremmisionProvider>(this IServiceCollection services)
		where TPremmisionProvider : class, IFieldPermissionProvider
	{
		services.AddSingleton<IFieldPermissionProvider,TPremmisionProvider>();
		services.AddSingleton<ISqlQueryBuilder, SqlQueryBuilder>();
		services.AddScoped<SqlExpressionValidator>();

		return services;
	}
}
