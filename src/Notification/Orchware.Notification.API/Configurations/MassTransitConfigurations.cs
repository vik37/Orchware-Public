using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Notification;
using IntegrationEvents.IntegrationEvents.Payment;
using MassTransit;
using Orchware.Notification.API.Infrastructure.EventHandlers;
using Orchware.Notification.API.Settings.Models;

namespace Orchware.Notification.API.Configurations;

public static class MassTransitConfigurations
{
	public static IServiceCollection AddMassTransitConfigurations(this IServiceCollection services, IConfiguration configuration)
	{
		var rabbitSettings = configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

		if (rabbitSettings == null)
		{
			throw new InvalidOperationException("RabbitMQSettings section not found in configuration.");
		}

		services.AddMassTransit(x =>
		{
			x.AddConsumer<InvoiceSentIntegrationEventConsumer>();
			x.AddConsumer<WarehousemanNotificationMessageIntegrationEventConsumer>();
			x.AddConsumer<ManagerNotificationMessageIntegrationEventConsumer>();
			x.AddConsumer<PaymentRejectedIntegrationEventConsumer>();
			x.AddConsumer<PaymentConfirmedIntegrationEventConsumer>();

			x.UsingRabbitMq((context, cfg) =>
			{
				cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));

				cfg.Host(rabbitSettings.Host, rabbitSettings.Port, rabbitSettings.VHost, h =>
				{
					h.Username(rabbitSettings.Username);
					h.Password(rabbitSettings.Password);
				});

				cfg.RegisterConsumerServices(context);
			});
		});

		services.AddHealthChecks()
				.AddRabbitMQ(
					rabbitConnectionString: $"amqp://{rabbitSettings.Username}:{rabbitSettings.Password}@{rabbitSettings.Host}:{rabbitSettings.Port}",
					name: "rabbitmq-check",
					tags: new[] { "ready" });

		return services;
	}

	private static IRabbitMqBusFactoryConfigurator RegisterConsumerServices(this IRabbitMqBusFactoryConfigurator conf,
		IBusRegistrationContext context)
	{
		conf.ReceiveEndpoint("notification-invoice-sent-queue", e =>
		{
			e.ConfigureConsumer<InvoiceSentIntegrationEventConsumer>(context);
			e.Bind<InvoiceSentIntegrationEvent>();
		});

		conf.ReceiveEndpoint("warehouseman-notification-message-sent-queue", e =>
		{
			e.ConfigureConsumer<WarehousemanNotificationMessageIntegrationEventConsumer>(context);
			e.Bind<WarehousemanNotificationMessageIntegrationEvent>();
		});

		conf.ReceiveEndpoint("manager-notification-message-sent-queue", e =>
		{
			e.ConfigureConsumer<ManagerNotificationMessageIntegrationEventConsumer>(context);
			e.Bind<ManagerNotificationMessageIntegrationEvent>();
		});

		conf.ReceiveEndpoint("payment-rejected-message-queue", e =>
		{
			e.ConfigureConsumer<PaymentRejectedIntegrationEventConsumer>(context);
			e.Bind<PaymentRejectedIntegrationEvent>();
		});

		conf.ReceiveEndpoint("payment-confirmed-message-queue", e =>
		{
			e.ConfigureConsumer<PaymentConfirmedIntegrationEventConsumer>(context);
			e.Bind<PaymentConfirmedIntegrationEvent>();
		});

		return conf;
	}
}
