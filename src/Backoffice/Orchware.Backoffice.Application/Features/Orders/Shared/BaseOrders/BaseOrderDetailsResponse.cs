using Orchware.Backoffice.Domain.Entities.Orders;
using System.Text.Json.Serialization;

namespace Orchware.Backoffice.Application.Features.Orders.Shared.BaseOrders;

public abstract class BaseOrderDetailsResponse
{
	public string OrderNumber { get; set; } = string.Empty;
	public DateTime OrderedDate { get; set; }

	public decimal TotalQuantityInKg { get; set; }

	public OrderStatusUIDto orderStatus { get; set; } = new();
	[JsonIgnore]
	public string Status
	{
		set => MapStatus(value);
	}

	private void MapStatus(string value)
	{
		var matchedStatus = OrderStatus.GetAllStatuses()
			.FirstOrDefault(x => x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

		orderStatus = matchedStatus != null
			? new OrderStatusUIDto { Value = matchedStatus.Value, Color = matchedStatus.Color }
			: new OrderStatusUIDto { Value = value, Color = "#cccccc" };
	}
}

public abstract class BaseOrderItemDto
{
	public string ShelfCode { get; set; } = string.Empty;

	public decimal Quantity { get; set; }

	public string UnitOfMeasure { get; set; } = string.Empty;
}

public abstract class BaseBuyerDto
{
	public string CompanyName { get; set; } = string.Empty;
	public string CompanyAddress { get; set; } = string.Empty;
	public string CompanyCity { get; set; } = string.Empty;
	public string CompanyLocation { get; set; } = string.Empty;
	public string CompanyPhoneNumber { get; set; } = string.Empty;
}