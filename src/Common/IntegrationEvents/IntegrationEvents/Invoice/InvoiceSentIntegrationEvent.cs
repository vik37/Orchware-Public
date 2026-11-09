using MassTransit;

namespace IntegrationEvents.IntegrationEvents.Invoice;

[EntityName("invoice-sent")]
public record InvoiceSentIntegrationEvent
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string OrderNumber { get; init; } = string.Empty;
	public string InvoiceUrl { get; set; } = string.Empty;
	public string? InstallmentCode { get; init; } = null;
}
