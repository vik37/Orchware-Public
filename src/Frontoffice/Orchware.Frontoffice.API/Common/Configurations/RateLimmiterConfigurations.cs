using System.Threading.RateLimiting;

namespace Orchware.Frontoffice.API.Common.Configurations;

public static class RateLimmiterConfigurations
{
	public static IServiceCollection AddRateLimmiterRegistration(this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = 429;

			options.AddPolicy("slide-by-ip", httpContext =>
			{
				var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
						 ?? httpContext.Connection.RemoteIpAddress?.ToString()
						 ?? "unknown";

				return RateLimitPartition.GetSlidingWindowLimiter(
					partitionKey: ip,
					factory: _ => new SlidingWindowRateLimiterOptions
					{
						PermitLimit = 400,
						Window = TimeSpan.FromMinutes(1),
						SegmentsPerWindow = 2,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 1
					});
			});

			options.AddPolicy("fixed-by-ip", httpContext =>
			{
				var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
						 ?? httpContext.Connection.RemoteIpAddress?.ToString()
						 ?? "unknown";

				return RateLimitPartition.GetFixedWindowLimiter(
					partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
					factory: _ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 5,
						Window = TimeSpan.FromSeconds(10),
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 1
					});
			});
				

			options.OnRejected = (ctx, token) =>
			{
				var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
				logger.LogWarning("Rate limit triggered for {IpAddress}", ctx.HttpContext.Connection.RemoteIpAddress);
				return ValueTask.CompletedTask;
			};
		});

		return services;
	}
}
