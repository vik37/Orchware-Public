using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Orchware.Notification.API.Configurations;

public static class AuthConfigurations
{
	public static IServiceCollection AddAuthRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.RequireHttpsMetadata = false;
				opt.Audience = configuration["Authentication:Audience"];
				opt.MetadataAddress = configuration["Authentication:MetadataAudience"]!;
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = configuration["Authentication:Issuer"],
					NameClaimType = "preferred_username",
					RoleClaimType = "role"
				};

				opt.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["access_token"];

						var path = context.HttpContext.Request.Path;
						if (!string.IsNullOrEmpty(accessToken) &&
							path.StartsWithSegments("/notificationHub"))
						{
							context.Token = accessToken;
						}

						return Task.CompletedTask;
					}
				};
			});

		return services;
	}
}
