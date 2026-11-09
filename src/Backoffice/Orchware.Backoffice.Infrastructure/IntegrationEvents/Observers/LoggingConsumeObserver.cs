using MassTransit;
using Microsoft.Extensions.Logging;

namespace Orchware.Backoffice.Infrastructure.IntegrationEvents.Observers;

public class LoggingConsumeObserver : IConsumeObserver
{
	private readonly ILogger<LoggingConsumeObserver> _logger;

	public LoggingConsumeObserver(ILogger<LoggingConsumeObserver> logger)
	{
		_logger = logger;
	}

	public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
	{
		if (exception is Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateException)
		{
			_logger.LogError(dbUpdateException,
				"A database update failed for message {MessageType}. MessageId={MessageId}",
				typeof(T).Name, context.MessageId);
		}
		else if (exception is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
		{
			_logger.LogError(exception,
				"Concurrency conflict for message {MessageType}. This is not a transient error. MessageId={MessageId}",
				typeof(T).Name, context.MessageId);
		}
		else
		{
			_logger.LogError(exception,
				"Message {MessageType} failed permanently. MessageId={MessageId}",
				typeof(T).Name, context.MessageId);
		}

		return Task.CompletedTask;
	}

	public Task PostConsume<T>(ConsumeContext<T> context) where T : class
	{	
		return Task.CompletedTask; 
	}

	public Task PreConsume<T>(ConsumeContext<T> context) where T : class
	{

			if (context.GetRetryCount() > 0)
			{
				_logger.LogWarning("Retrying {MessageType}, attempt {Attempt}",
					typeof(T).Name, context.GetRetryCount());
			}

		return Task.CompletedTask;
	}
}
