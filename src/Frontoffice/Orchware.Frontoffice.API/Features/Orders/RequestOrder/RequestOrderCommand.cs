using MediatR;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;

namespace Orchware.Frontoffice.API.Features.Orders.RequestOrder;

public record RequestOrderCommand : IRequest<Unit>
{
	public PaymentDto Payment { get; init; } = new();
	public IReadOnlyCollection<OrderDetailsDto>? Details { get; init; }
}

public class OrderDetailsDto
{
	public Guid TempId { get; set; }
	public ProductDto Product { get; set; } = new ProductDto();
	public int ProductId { get; set; }
	public decimal Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }
}

public class PaymentDto
{
	public DateTime PaymentDate { get; set; }
	public bool WillBePaidInFull { get; set; }
	public int? NumberOfInstallments { get; set; }
}