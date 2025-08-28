namespace Orchware.Frontoffice.API.Domain;

public class Order : BaseAuditableEntityGuid
{
	public OrderStatus Status { get; set; } = OrderStatus.Pending;
	public int CompanyId { get; set; }
	public Company Company { get; set; }
	public Guid UserId { get; set; }
	public User User { get; set; }
	public DateTime? OrderDate { get; set; }
	public decimal Amount { get; set; }
	public decimal TotalAmount { get; set; }
	public int? Discount { get; set; }
	public string OrderNumber { get; private set; }

	public ICollection<OrderDetails> OrderDetails { get; set; }
		

	public Order()
	{
		OrderNumber = $"#ON{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString().Substring(0, 8)}";
		OrderDetails = new List<OrderDetails>();
		User = new();
		Company = new();
	}
}

public class OrderStatus
{
	public string Value { get; }
	public string Color { get; }

	private OrderStatus(string value, string color)
	{
		Value = value;
		Color = color;
	}

	public static OrderStatus Pending => new OrderStatus("Pending", "#f5ef38");
	public static OrderStatus Requested => new OrderStatus("Requested", "#bed973");
	public static OrderStatus Confirmed => new OrderStatus("Confirmed", "#fa9052");
	public static OrderStatus InPreparation => new OrderStatus("InPreparation", "#18f2e7");
	public static OrderStatus AwaitingDispatch => new OrderStatus("AwaitingDispatch", "#d920fa");
	public static OrderStatus Approved => new OrderStatus("Approved", "#57f75c");
	public static OrderStatus Cancelled => new OrderStatus("Cancelled", "#f00202");

	public static IEnumerable<OrderStatus> GetAllStatuses() => new[]
	{
		Pending, Requested, Confirmed, InPreparation, AwaitingDispatch, Approved, Cancelled
	};
}