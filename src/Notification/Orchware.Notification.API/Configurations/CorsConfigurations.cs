namespace Orchware.Notification.API.Configurations;

public static class CorsConfigurations
{
	public static IServiceCollection AddCorsRegistration(this IServiceCollection services)
	{
		services.AddCors(option =>
		{
			option.AddPolicy("OrchwareNOPoliciesDev", policy =>
			{
				policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://127.0.0.1:5500")
					 .AllowAnyHeader()
					 .AllowAnyMethod()
					 .AllowCredentials()
					 .WithExposedHeaders("Location");
			});
		});

		services.AddCors(option =>
		{
			option.AddPolicy("OrchwareNOPoliciesProd", policy =>
			{
				policy.WithOrigins("http://orchwarefrontend", "https://viktor-showcase.dev", "https://www.viktor-showcase.dev")
					 .AllowAnyHeader()
					 .WithMethods("GET", "POST", "PUT", "OPTIONS")
					 .AllowCredentials()
					 .WithExposedHeaders("Location");
			});
		});

		return services;
	}
}
