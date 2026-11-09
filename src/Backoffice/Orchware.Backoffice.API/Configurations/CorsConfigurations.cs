namespace Orchware.Backoffice.API.Configurations;

public static class CorsConfigurations
{
	public static IServiceCollection AddCorsRegistration(this IServiceCollection services)
	{
		services.AddCors(option =>
		{
			option.AddPolicy("OrchwareBOPoliciesDev", policy =>
			{
				policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
					 .AllowAnyHeader()
					 .AllowAnyMethod()
					 .AllowCredentials()
					 .WithExposedHeaders("Content-Disposition");
			});
		});

		services.AddCors(option =>
		{
			option.AddPolicy("OrchwareBOPoliciesProd", policy =>
			{
				policy.WithOrigins("http://orchwarefrontend", "https://viktor-showcase.dev", "https://www.viktor-showcase.dev")
					 .AllowAnyHeader()
					 .WithMethods("GET", "POST", "PUT")
					 .AllowCredentials()
					 .WithExposedHeaders("Content-Disposition");
			});
		});

		return services;
	}
}
