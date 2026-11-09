using Orchware.Backoffice.Application.Features.Orders.Shared.BaseOrders;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.WarehousemanOrderDetails;

public sealed class WarehousemanOrderDetailsResponse : BaseOrderDetailsResponse
{
	public string BuyerCompanyName { get; set; } = string.Empty;
	public string BuyerCompanyAddress { get; set; } = string.Empty;
	public string BuyerCompanyCity { get; set; } = string.Empty;
	public string BuyerCompanyLocation { get; set; } = string.Empty;
	public string BuyerCompanyPhoneNumber { get; set; } = string.Empty;
	public List<OrderItemsDto> Items { get; set; } = new();
	
}

public sealed class OrderItemsDto : BaseOrderItemDto
{
	public OrderedProductPreviewDto Product { get; set; } = new();
}

public sealed class OrderedProductPreviewDto
{
	public string Name { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
}

internal sealed class WarehousemanOrderDetailsSqlDto
{
	public string OrderNumber { get; set; } = string.Empty;

	public DateTime OrderedDate { get; set; }

	public decimal TotalQuantityInKg { get; set; }

	public string Status { get; set; } = string.Empty;

	public string CompanyName { get; set; } = string.Empty;

	public string CompanyAddress { get; set; } = string.Empty;

	public string CompanyCity { get; set; } = string.Empty;

	public string CompanyLocation { get; set; } = string.Empty;

	public string CompanyPhoneNumber { get; set; } = string.Empty;
}

internal sealed class WarehousemanOrderItemSqlDto
{
	public string ShelfCode { get; set; } = string.Empty;

	public decimal Quantity { get; set; }

	public string UnitOfMeasure { get; set; } = string.Empty;

	public string ProductName { get; set; } = string.Empty;

	public string ProductImage { get; set; } = string.Empty;
}
