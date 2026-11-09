
using backoffice_worker.Abstractions;

namespace backoffice_worker.Configurations;

public static class ScheduledServiceExtensions
{
	public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options)
		where T : CronJobWorker
	{
		var config = new ScheduleConfig<T>();
		options.Invoke(config);

		services.AddSingleton<IScheduleConfig<T>>(config);

		services.AddHostedService<T>();

		return services;
	}
}
