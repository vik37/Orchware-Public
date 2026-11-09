using System.Text.Json.Serialization;

namespace Orchware.Frontoffice.API.Infrastructure.Cache.Models;

public class PendingOrderCacheModel
{
	public OrderStatusCache Status { get; }

	public List<OrderDetailsCache> Details { get; }

	public decimal Amount => Details.Sum(d => d.PricePerUnit * (decimal)d.Quantity);

	public PendingOrderCacheModel(OrderStatusCache status)
	{
		Status = status ?? throw new ArgumentNullException(nameof(status));
		Details = new List<OrderDetailsCache>();
	}

	[JsonConstructor]
	public PendingOrderCacheModel(OrderStatusCache status, List<OrderDetailsCache> details)
	{
		Status = status ?? throw new ArgumentNullException(nameof(status));
		Details = details ?? new List<OrderDetailsCache>();
	}

	public void AddOrUpdateDetails(IReadOnlyCollection<OrderDetailsCache> newDetails)
	{
		foreach (var newItem in newDetails)
		{
			var existing = Details.FirstOrDefault(x => x.ProductId == newItem.ProductId);
			if (existing is not null)
			{
				existing.Quantity += newItem.Quantity;
				existing.CalculateTotalPrice();
			}
			else
			{
				newItem.TempId = Guid.NewGuid();
				Details.Add(newItem);
			}
		}
	}

	public void DeleteOrderDetails(Guid tempId)
	{
		Details.RemoveAll(x => x.TempId == tempId);
	}
}

public class OrderStatusCache
{
	public string Value { get; }
	public string Color { get; }

	[JsonConstructor]
	public OrderStatusCache(string value, string color)
	{
		Value = value;
		Color = color;
	}
}

public class OrderDetailsCache
{
	public Guid TempId { get; set; }
	public ProductCache? Product { get; set; }
	public int ProductId { get; set; }
	public decimal Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }

	//public OrderDetailsCache()
	//{
	//	this.TotalPrice = this.PricePerUnit * this.Quantity;
	//}

	public void CalculateTotalPrice()
	{
		TotalPrice = PricePerUnit * Quantity;
	}
}

public class ProductCache
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string SeasonalFruits { get; set; } = string.Empty;
	public float AvailableQuantity { get; set; }
	public decimal Price { get; set; }
	public int MinQuantity { get; set; }
	public string Units { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
}
