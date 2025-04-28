namespace Orchware.Frontoffice.API.Domain;

public class OrderDetails : Base
{
	public Guid OrderId { get; set; }
	public Order Order { get; set; } = new Order();
	public int ProductId { get; set; }
	public Product Product { get; set; } = new();
	public float Quantity { get; set; }
	public decimal PricePerUnit { get; set; }
	public string UnitOfMeasure { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }
}
