using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Orchware.Backoffice.API.Configurations;

public static class SwaggerConfigurations
{
	public static IServiceCollection AddSwaggerRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSwaggerGen(opt =>
		{
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

			opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

			opt.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

			opt.AddSecurityDefinition("keycloak", new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.OAuth2,
				Flows = new OpenApiOAuthFlows
				{
					AuthorizationCode = new OpenApiOAuthFlow
					{
						AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUri"]!),
						TokenUrl = new Uri(configuration["Keycloak:TokenUrl"]!),
						Scopes = new Dictionary<string, string>
						{
							{ "openid", "openid" },
							{ "profile", "profile" }
						}
					}
				}
			});

			var securityRequirement = new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Id = "keycloak",
							Type = ReferenceType.SecurityScheme
						},
						In = ParameterLocation.Header,
						Name = "Bearer",
						Scheme = "Bearer"
					},
					[]
				}
			};

			opt.AddSecurityRequirement(securityRequirement);
		});

		return services;
	}
}
