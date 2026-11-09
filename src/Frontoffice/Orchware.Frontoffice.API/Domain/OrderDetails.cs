namespace Orchware.Frontoffice.API.Domain;

public class OrderDetails : BaseInt
{
	public Guid OrderId { get; set; }
	public Order? Order { get; set; }
	public int ProductId { get; set; }
	public decimal Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; private set; }

	public OrderDetails(){}

	public OrderDetails(int productId,decimal quantity, decimal pricePerUnit, string unitOfMeasure)
	{
		ProductId = productId;
		Quantity = quantity;
		PricePerUnit = pricePerUnit;
		UnitOfMeasure = unitOfMeasure;
		CalculateTotalPrice();
	}

	public void CalculateTotalPrice()
	{
		TotalPrice = PricePerUnit * Quantity;
	}
}
