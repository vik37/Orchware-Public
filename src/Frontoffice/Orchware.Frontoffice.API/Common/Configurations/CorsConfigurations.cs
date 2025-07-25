namespace Orchware.Frontoffice.API.Common.Configurations;

public static class CorsConfigurations
{
	public static IServiceCollection AddCorsRegistration(this IServiceCollection services)
	{
		services.AddCors(option =>
		{
			option.AddPolicy("OrchwareFOPoliciesDev", policy =>
			{
				policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
					 .AllowAnyHeader()
					 .AllowAnyMethod()
					 .AllowCredentials();
			});
		});

		return services;
	}
}
