using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Shipping;

public class Shipment : AggregateRoot<Guid>
{
	public Guid OrderId { get; set; }
	public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.InTransit;
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }
}
