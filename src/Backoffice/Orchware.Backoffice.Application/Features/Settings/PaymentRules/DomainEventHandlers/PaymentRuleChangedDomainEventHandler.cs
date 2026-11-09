using MediatR;
using Orchware.Backoffice.Application.Features.Shared.Contract.IntegrationEventProducers;
using Orchware.Backoffice.Domain.Events;

namespace Orchware.Backoffice.Application.Features.Settings.PaymentRules.DomainEventHandlers;

public class PaymentRuleChangedDomainEventHandler : INotificationHandler<PaymentRuleChangedDomainEvent>
{
	private readonly IIntegrationEventProducer _integrationEventProducer;

	public PaymentRuleChangedDomainEventHandler(IIntegrationEventProducer integrationEventProducer)
	{
		_integrationEventProducer = integrationEventProducer;
	}

	public async Task Handle(PaymentRuleChangedDomainEvent notification, CancellationToken cancellationToken)
	{
		await _integrationEventProducer.PaymentRuleChanged(notification.Rules.MaxAllowedInstallments, notification.Rules.GracePeriod,
			notification.Rules.MaximumPaymentPeriod, notification.Rules.MinimumOrderAmountForDiscount, notification.Rules.OrderAmountDiscountPercentage,
			notification.Rules.PayImmediatelyDiscountPercentage);
	}
}
