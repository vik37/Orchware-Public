using Orchware.Backoffice.Domain.Primitives;
using System.Text.Json;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public sealed class OrderDetails : Entity<int>
{
	public Guid OrderId { get; set; }

	private string _shelfCode = string.Empty;
	public string ShelfCode { get => _shelfCode; }

	private int _shelfProductId;
	public int ShelfProductId { get => _shelfProductId; }

	private string _product = string.Empty;
	public OrderedProduct Product { get => string.IsNullOrEmpty(_product)?new( ) :
			JsonSerializer.Deserialize<OrderedProduct>(_product)?? new(); }

	private decimal _quantity;
	public decimal Quantity { get => _quantity; }

	private decimal _pricePerUnit;
	public decimal PricePerUnit { get => _pricePerUnit; }

	private string _unitOfMeasure = string.Empty;
	public string UnitOfMeasure { get => _unitOfMeasure; }

	private decimal _totalPrice;
	public decimal TotalPrice { get => _totalPrice; }

	public OrderDetails(){}

	public OrderDetails(OrderedProduct orderedProduct, decimal quantity, decimal pricePerUnit, 
			string unitOfMeasure, decimal totalPrice)
	{
		_product = orderedProduct is null ? throw new ArgumentNullException(nameof(orderedProduct)) : JsonSerializer.Serialize(orderedProduct);
		_quantity = quantity;
		_pricePerUnit = pricePerUnit;
		_unitOfMeasure = unitOfMeasure;
		_totalPrice = totalPrice;
	}

	public void AddShelfInfo(string shelfCode, int shelfProductId)
	{
		_shelfCode = shelfCode; 
		_shelfProductId= shelfProductId;
	}
}

public class OrderedProduct
{
	public string Name { get; set; } = string.Empty;
	public string SeasonalFruits { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public string Units { get; set; } = string.Empty;
	public string Image { get; set; } = string.Empty;
}