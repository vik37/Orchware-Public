using Orchware.Backoffice.Domain.Primitives;
using System.Text.Json;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public sealed class OrderDetails : Entity<int>
{
	public Guid OrderId { get; set; }
	public Order Order { get; set; } = new();
	public string Products { get; set; } = string.Empty;
	public float Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }

	public List<OrderedProduct> OrderedProducts 
	{ 
		get => string.IsNullOrEmpty(Products) ? new()
			: JsonSerializer.Deserialize<List<OrderedProduct>>(Products) ?? new(); 
		set => Products = JsonSerializer.Serialize<List<OrderedProduct>>(value); 
	}
}

public class OrderedProduct
{
	public string SeasonalFruits { get; set; } = string.Empty;
	public float AvailableQuantity { get; set; }
	public decimal Price { get; set; }
	public int MinQuantity { get; set; }
	public string Units { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
}