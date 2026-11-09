using MediatR;
using Orchware.Frontoffice.API.Features.Orders.Shared.Models;

namespace Orchware.Frontoffice.API.Features.Orders.PendingOrderCreation;

public record PendingOrderCreationCommand : IRequest<Unit>
{
	public IReadOnlyCollection<OrderDetailsDto> Details { get; init; } = new List<OrderDetailsDto>();
}

public class OrderDetailsDto
{
	public ProductDto Product { get; set; } = new ProductDto();
	public decimal Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
}