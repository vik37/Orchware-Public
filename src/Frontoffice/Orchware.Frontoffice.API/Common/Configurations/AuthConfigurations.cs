using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Orchware.Frontoffice.API.Common.Configurations;

public static class AuthConfigurations
{
	public static IServiceCollection AddAuthRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthorization(opt => {
			opt.AddPolicy("NonEmployeeUser", policy => 
			{
				policy.RequireAssertion(ctx =>
				{
					var roles = ctx.User.FindAll(ClaimTypes.Role).Select(r => r.Value);
					var hasUserRole = roles.Contains("user");
					var isEmployee = roles.Any(r => new[] { "manager", "warehouseman" }.Contains(r));

					return hasUserRole && !isEmployee;
				});
			});
		});

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
