using Cronos;

namespace backoffice_worker.Abstractions;

public abstract class CronJobWorker : BackgroundService
{
	private readonly CronExpression _expression;
	private readonly TimeZoneInfo _timeZoneInfo;
	private readonly IServiceProvider _serviceProvider;
	protected readonly ILogger<CronJobWorker> _logger;

	protected CronJobWorker(string cronExpression, TimeZoneInfo timeZoneInfo, IServiceProvider serviceProvider, ILogger<CronJobWorker> logger)
	{
		_expression = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
		_timeZoneInfo = timeZoneInfo;
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var now = DateTimeOffset.Now;
			var next = _expression.GetNextOccurrence(now, _timeZoneInfo);
			if (!next.HasValue) continue;

			var delay = next.Value - now;
			await Task.Delay(delay, stoppingToken);

			if (stoppingToken.IsCancellationRequested) continue;

			try
			{
				using var scope = _serviceProvider.CreateScope();
				await DoWork(scope, stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, nameof(ExecuteAsync));
			}
		}
	}
		public abstract Task DoWork(IServiceScope scope, CancellationToken cancellationToken);
}
