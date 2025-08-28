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
					 .AllowCredentials()
					 .WithExposedHeaders("Location");
			});
		});

		services.AddCors(option =>
		{
			option.AddPolicy("OrchwareFOPoliciesProd", policy =>
			{
				policy.WithOrigins("http://orchwarefrontend", "https://viktor-showcase.dev", "https://www.viktor-showcase.dev")
					 .AllowAnyHeader()
					 .WithMethods("GET", "POST", "PUT")
					 .AllowCredentials()
					 .WithExposedHeaders("Location");
			});
		});

		return services;
	}
}
