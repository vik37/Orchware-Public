
namespace backoffice_worker.Abstractions;

public interface IScheduleConfig<T>
{
	string CronExpression { get; set; }
	TimeZoneInfo TimeZoneInfo { get; set; }
}
