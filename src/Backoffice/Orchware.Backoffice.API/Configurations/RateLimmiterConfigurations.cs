using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Orchware.Backoffice.API.Configurations;

public static class RateLimmiterConfigurations
{
	public static IServiceCollection AddRateLimmiterRegistration(this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = 429;

			options.AddPolicy("limit-by-user-query", httpContext =>
			{
				var userId = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier)
							 ?? httpContext.User?.FindFirstValue("sub");

				var partitionKey = !string.IsNullOrEmpty(userId)
					? userId
					: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

				return RateLimitPartition.GetSlidingWindowLimiter(
					partitionKey: partitionKey,
					factory: _ => new SlidingWindowRateLimiterOptions
					{
						PermitLimit = 200,
						Window = TimeSpan.FromMinutes(1),
						SegmentsPerWindow = 2,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 1
					});
			});

			options.AddPolicy("limit-by-user-command", httpContext =>
			{
				var userId = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? httpContext.User?.FindFirstValue("sub");
				var partitionKey = !string.IsNullOrEmpty(userId) ? userId : httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

				return RateLimitPartition.GetSlidingWindowLimiter(
					partitionKey: partitionKey,
					factory: _ => new SlidingWindowRateLimiterOptions
					{
						PermitLimit = 20,
						Window = TimeSpan.FromMinutes(1),
						SegmentsPerWindow = 2,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						QueueLimit = 0
					});
			});

			options.OnRejected = (ctx, token) =>
			{
				var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
				var userId = ctx.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

				if (!string.IsNullOrEmpty(userId))
				{
					logger.LogWarning("Rate limit triggered for User ID {UserId}", userId);
				}
				else
				{
					logger.LogWarning("Rate limit triggered for IP {IpAddress}", ctx.HttpContext.Connection.RemoteIpAddress);
				}

				return ValueTask.CompletedTask;
			};
		});

		return services;
	}
}
