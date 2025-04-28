using Orchware.Backoffice.Domain.Primitives;
using System.Text.Json;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public sealed class Order : AggregateRoot<Guid>
{
	public OrderStatus Status { get; set; } = OrderStatus.Pending;
	public int CompanyId { get; set; }
	public Company Company { get; set; } = new();
	public DateTime? OrderDate { get; set; }
	public decimal Amount { get; set; }
	public decimal TotalAmount { get; set; }
	public int? Discount { get; set; }
	public string OrderNumber { get; set; } = string.Empty;
	public string Payment { get; set; } = string.Empty;
	public DateTime CreatedDate { get; set; }
	public DateTime? ModifiedDate { get; set; }

	public PaymentDetails PaymentDetails
	{
		get => string.IsNullOrEmpty(Payment) ? new() 
			: JsonSerializer.Deserialize<PaymentDetails>(Payment) ?? new();
		set => Payment = JsonSerializer.Serialize<PaymentDetails>(value);
	}

	private List<OrderDetails> _orderDetails = new();
	public IReadOnlyCollection<OrderDetails> OrderDetails => _orderDetails.AsReadOnly();

	public void AddDetails(OrderDetails details)
		=> _orderDetails.Add(details);
}

public class PaymentDetails
{
	public string CustomerId { get; set; } = string.Empty;
	public Guid OrderId { get; set; }
	public DateTime PaymentDate { get; set; }
	public bool PayImmediately { get; set; } = true;
	public DateTime? SelectedPaymentDate { get; set; }
	public int MaxsimumAllowedNumberOfMonths { get; set; }
}