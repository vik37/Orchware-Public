using IntegrationEvents.IntegrationEvents.Invoice;
using IntegrationEvents.IntegrationEvents.Orders;
using IntegrationEvents.IntegrationEvents.Payment;
using IntegrationEvents.IntegrationEvents.PaymentRules;
using MassTransit;
using Orchware.Frontoffice.API.Common.Models;
using Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.EventConsumers;
using Orchware.Frontoffice.API.Infrastructure.IntegrationEventHandlers.Filter;

namespace Orchware.Frontoffice.API.Common.Configurations;

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
			x.AddConsumer<OrderConfirmedIntegrationEventConsumer>();
			x.AddConsumer<InvoiceSentIntegrationEventConsumer>();
			x.AddConsumer<OrderApprovedIntegrationEventConsumer>();
			x.AddConsumer<OrderPackedIntegrationEventConsumer>();
			x.AddConsumer<EmployeeCancelledOrderEventConsumer>();
			x.AddConsumer<PaymentRequestedIntegrationEventConsumer>();

			x.AddConsumer<PaymentRuleChangedIntegrationEventConsumer>();

			x.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host(rabbitSettings.Host, rabbitSettings.Port, rabbitSettings.VHost, h =>
				{
					h.Username(rabbitSettings.Username);
					h.Password(rabbitSettings.Password);
				});

				//cfg.ConfigureEndpoints(context);
				cfg.RegisterConsumerServices(context);
			});
		});

		services.AddScoped(typeof(IdempotencyFilter<>));

		return services;
	}

	private static IRabbitMqBusFactoryConfigurator RegisterConsumerServices(this IRabbitMqBusFactoryConfigurator conf,
		IBusRegistrationContext context)
	{
		conf.ReceiveEndpoint("order-confirmed-queue", e =>
		{
			e.ConfigureConsumer<OrderConfirmedIntegrationEventConsumer>(context);
			e.Bind<OrderConfirmedIntegrationEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);
		});

		conf.ReceiveEndpoint("frontoffice-invoice-sent-queue", e =>
		{
			e.ConfigureConsumer<InvoiceSentIntegrationEventConsumer>(context);
			e.Bind<InvoiceSentIntegrationEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);
		});

		conf.ReceiveEndpoint("order-approved-queue", e =>
		{
			e.ConfigureConsumer<OrderApprovedIntegrationEventConsumer>(context);
			e.Bind<OrderApprovedIntegrationEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);
		});

		conf.ReceiveEndpoint("order-packed-queue", e =>
		{
			e.ConfigureConsumer<OrderPackedIntegrationEventConsumer>(context);
			e.Bind<OrderPackedIntegrationEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);
		});

		conf.ReceiveEndpoint("employee-cancelled-order-queue", e =>
		{
			e.ConfigureConsumer<EmployeeCancelledOrderEventConsumer>(context);
			e.Bind<EmployeeCancelledOrderEvent>();

			e.UseConsumeFilter(typeof(IdempotencyFilter<>), context);
		});

		conf.ReceiveEndpoint("payment-requested-queue", e =>
		{
			e.ConfigureConsumer<PaymentRequestedIntegrationEventConsumer>(context);
			e.Bind<PaymentRequestedIntegrationEvent>();
		});

		conf.ReceiveEndpoint("payment-rule-changed-queue", e =>
		{
			e.ConfigureConsumer<PaymentRuleChangedIntegrationEventConsumer>(context);
			e.Bind<PaymentRuleChangedIntegrationEvent>();
		});

		return conf;
	}
}
