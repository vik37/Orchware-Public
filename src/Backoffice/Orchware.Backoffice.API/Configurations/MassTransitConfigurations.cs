using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Orders;
using IntegrationEvents.IntegrationEvents.Payment;
using IntegrationEvents.Orders;
using MassTransit;
using Orchware.Backoffice.API.Models;
using Orchware.Backoffice.Infrastructure.IntegrationEvents.EventConsumers;
using Orchware.Backoffice.Infrastructure.IntegrationEvents.Filter;
using Orchware.Backoffice.Infrastructure.IntegrationEvents.Observers;

namespace Orchware.Backoffice.API.Configurations;

public static class MassTransitConfigurations
{
	public static IServiceCollection AddMassTransitRegistration(this IServiceCollection services, IConfiguration configuration)
	{
		var rabbitSettings = configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

		if (rabbitSettings == null)
		{
			throw new InvalidOperationException("RabbitMQSettings section not found in configuration.");
		}

		services.AddMassTransit(cf =>
		{
			cf.AddConsumer<OrderRequestedIntegrationEventConsumer>();
			cf.AddConsumer<InvoiceSentIntegrationEventConsumer>();
			cf.AddConsumer<CustomerCancelledOrderEventConsumer>();
			cf.AddConsumer<PaymentConfirmedIntegrationEventConsumer>();

			cf.UsingRabbitMq((context, conf) =>
			{
				conf.Host(rabbitSettings.Host, rabbitSettings.Port, rabbitSettings.VHost, host =>
				{
					host.Username(rabbitSettings.Username);
					host.Password(rabbitSettings.Password);
				});

				conf.RegisterConsumerServices(context);

				conf.ConnectConsumeObserver(new LoggingConsumeObserver(context.GetRequiredService<ILogger<LoggingConsumeObserver>>()));
			});
		});

		services.AddScoped(typeof(IdempotencyFilter<>));

		return services;
	}

	private static IRabbitMqBusFactoryConfigurator RegisterConsumerServices(this IRabbitMqBusFactoryConfigurator conf, 
		IBusRegistrationContext context)
	{
		conf.ReceiveEndpoint("order-requested-queue", e =>
		{
			e.ConfigureConsumer<OrderRequestedIntegrationEventConsumer>(context);
			e.Bind<OrderRequestedIntegrationEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);

			e.UseMessageRetry(r =>
			{
				r.Interval(5, TimeSpan.FromSeconds(5));

				r.Ignore<ArgumentException>();
				r.Ignore<ArgumentNullException>();
			});
		});

		conf.ReceiveEndpoint("backoffice-invoice-sent-queue", e =>
		{
			e.ConfigureConsumer<InvoiceSentIntegrationEventConsumer>(context);
			e.Bind<InvoiceSentIntegrationEvent>();

			//e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);

			e.UseMessageRetry(r =>
			{
				r.Interval(5, TimeSpan.FromSeconds(5));

				r.Ignore<ArgumentException>();
				r.Ignore<ArgumentNullException>();
			});
		});

		conf.ReceiveEndpoint("customer-cancelled-order-queue", e =>
		{
			e.ConfigureConsumer<CustomerCancelledOrderEventConsumer>(context);
			e.Bind<CustomerCancelledOrderEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);

			e.UseMessageRetry(r =>
			{
				r.Interval(5, TimeSpan.FromSeconds(5));

				r.Ignore<ArgumentException>();
				r.Ignore<ArgumentNullException>();
			});
		});

		conf.ReceiveEndpoint("payment-confirmed-queue", e =>
		{
			e.ConfigureConsumer<PaymentConfirmedIntegrationEventConsumer>(context);
			e.Bind<PaymentConfirmedIntegrationEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);

			e.UseMessageRetry(r =>
			{
				r.Interval(5, TimeSpan.FromSeconds(5));

				r.Ignore<ArgumentException>();
				r.Ignore<ArgumentNullException>();
			});
		});

		return conf;
	}
}
