using Orchware.Backoffice.Application.Features.Orders.Shared.BaseOrders;

namespace Orchware.Backoffice.Application.Features.Orders.Queries.ManagerOrderDetails;

public class ManagerOrderDetailsResponse : BaseOrderDetailsResponse
{
	public List<OrderItemsDto> Items { get; set; } = new();
	public int? Discount { get; set; }
	public decimal Amount { get; set; }
	public decimal TotalAmount { get; set; }

	public BuyerDto Buyer { get; set; } = new();
	public PaymentDto Payment { get; set; } = new();

	public string? CancellationReason { get; set; }
	public string? CancelledBy { get; set; }
}

public class BuyerDto : BaseBuyerDto
{
	public string PersonPlacedOrderName { get; set; } = string.Empty;
	public string PersonPlacedOrderEmailContact { get; set; } = string.Empty;
	public string CompanyEmail { get; set; } = string.Empty;
}

public class OrderItemsDto : BaseOrderItemDto
{
	public OrderedProductSnapshotDto Product { get; set; } = new();

	public decimal PricePerUnit { get; set; }

	public decimal TotalPrice { get; set; }
}

public sealed class OrderedProductSnapshotDto
{
	public string Name { get; set; } = string.Empty;

	public string SeasonalFruits { get; set; } = string.Empty;

	public decimal Price { get; set; }

	public string Units { get; set; } = string.Empty;

	public string Image { get; set; } = string.Empty;
}

public class PaymentDto
{
	public string PaymentNumber { get; set; } = string.Empty;
	public bool WillBePaidInFull { get; set; }
	public decimal TotalAmount { get; set; }
	public DateTime? Deadline { get; set; }
	public bool? HasPaymentInvoice { get; set; }
	public bool IsFullySettled { get; set; }

	public decimal TotalRemainingAmount { get; set; } 
	public DateTime? NextIncomingPaymentDate { get; set; }
	public bool HasOverdueInstallments { get; set; }
	public decimal OverdueBalance { get; set; }
	public bool IsFirstInstallmentPaid { get; set; }
	public decimal InstallmentAmount { get; set; }
	public string? PaidInstallmentsStatus { get; set; }
	public int? TotalInstallmentsCount { get; set; }
	public int? InstallmentsWithInvoiceCount { get; set; }
}

internal sealed class ManagerOrderDetailsSqlDto
{
	public string OrderNumber { get; set; } = string.Empty;

	public DateTime OrderedDate { get; set; }

	public int? Discount { get; set; }

	public decimal Amount { get; set; }

	public decimal TotalAmount { get; set; }

	public decimal TotalQuantityInKg { get; set; }

	public string Status { get; set; } = string.Empty;

	public string PersonPlacedOrderName { get; set; } = string.Empty;

	public string PersonPlacedOrderEmailContact { get; set; } = string.Empty;

	public string CompanyName { get; set; } = string.Empty;

	public string CompanyAddress { get; set; } = string.Empty;

	public string CompanyEmail { get; set; } = string.Empty;

	public string CompanyCity { get; set; } = string.Empty;

	public string CompanyLocation { get; set; } = string.Empty;

	public string CompanyPhoneNumber { get; set; } = string.Empty;

	public string? CancellationReason { get; set; }
	public int? CancelledBy { get; set; }
}

internal sealed class ManagerOrderItemSqlDto
{
	public string ShelfCode { get; set; } = string.Empty;

	public decimal Quantity { get; set; }

	public string UnitOfMeasure { get; set; } = string.Empty;

	public decimal PricePerUnit { get; set; }

	public decimal TotalPrice { get; set; }

	public string ProductName { get; set; } = string.Empty;

	public string ProductImage { get; set; } = string.Empty;

	public string ProductSeasonalFruits { get; set; } = string.Empty;

	public decimal ProductPrice { get; set; }

	public string ProductUnits { get; set; } = string.Empty;
}

