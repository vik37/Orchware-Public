using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Orchware.Backoffice.API.Configurations;

public static class AuthConfigurations
{
	public static IServiceCollection AddAuthRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthorization();

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.RequireHttpsMetadata = false;
				opt.Audience = configuration["Authentication:Audience"];
				opt.MetadataAddress = configuration["Authentication:MetadataAudience"]!;
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = configuration["Authentication:Issuer"],
					NameClaimType = "preferred_username"
				};
			});

		return services;
	}
}
