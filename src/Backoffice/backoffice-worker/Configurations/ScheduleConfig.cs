using backoffice_worker.Abstractions;

namespace backoffice_worker.Configurations;

public class ScheduleConfig<T> : IScheduleConfig<T>
{
	public string CronExpression { get; set; } = string.Empty;
	public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
}
