using Orchware.Backoffice.Domain.Enums;
using Orchware.Backoffice.Domain.Events;
using Orchware.Backoffice.Domain.Events.Contract;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public sealed class Order : AggregateRoot<Guid>
{
	private string _orderNumber;
	public string OrderNumber { get => _orderNumber; }

	public OrderStatus Status { get; private set; }
	public VisibilityLevel VisibilityLevel { get; private set; }

	private DateTime _orderedDate;
	public DateTime OrderedDate { get => _orderedDate; }

	public Buyer Buyer { get; private set; }

	private decimal _amount;
	public decimal Amount { get => _amount; }

	private decimal _totalAmount;
	public decimal TotalAmount { get => _totalAmount; }

	private int? _discount;
	public int? Discount { get => _discount; }

	private string? _cancellationReason;
	public string? CancellationReason { get => _cancellationReason; }

	private CancelledBy _cancelledBy = CancelledBy.None;
	public CancelledBy CancelledBy { get => _cancelledBy; }

	private PaymentInfo _paymentInfo = new();
	public PaymentInfo PaymentInfo => _paymentInfo;

	private DateTime _createdDate;
	public DateTime CreatedDate { get => _createdDate; }

	private DateTime? _modifiedDate;
	public DateTime? ModifiedDate { get => _modifiedDate; }

	private List<OrderDetails> _orderDetails = new();
	public IReadOnlyCollection<OrderDetails> OrderDetails => _orderDetails.AsReadOnly();

#nullable disable
	public Order(){}
#nullable enable

	public Order(string orderNumber, DateTime orderedDate, decimal amount, decimal totalAmount, int? discount,
		PaymentInfo paymentInfo, Buyer buyer, IEnumerable<OrderDetails> orderDetails)
	{
		_orderNumber = orderNumber;
		_orderedDate = orderedDate;
		_amount = amount;
		_totalAmount = totalAmount;
		_discount = discount;

		if (paymentInfo is null) throw new ArgumentNullException(nameof(paymentInfo));

		_paymentInfo = paymentInfo;

		if(buyer is null) throw new ArgumentNullException(nameof(buyer));
		
		Buyer = buyer;

		if(orderDetails is null || orderDetails.Count() < 1) throw new ArgumentNullException(nameof(orderDetails));

		_orderDetails = orderDetails.ToList();

		_createdDate = DateTime.UtcNow;
		Status = OrderStatus.Requested;
		VisibilityLevel = VisibilityLevel.Manager;
	}

	public void SetOrderDetailsShelfInfo(Dictionary<string, (string shelfCode, int shelfProductId)> shelfDescription)
	{
		foreach(var detail in this._orderDetails)
		{
			if(shelfDescription.TryGetValue(detail.Product.Name, out var shelfInfo))
			{
				detail.AddShelfInfo(shelfInfo.shelfCode, shelfInfo.shelfProductId);
			}
		}
	}

	public void CancelOrder(string reason, CancelledBy cancelledBy)
	{
		if (!OrderStatus.CanTransition(Status, OrderStatus.Cancelled))
			throw new InvalidOperationException($"An order with status {Status.Value} cannot be canceled.");

		string previousStatus = Status.Value;
		Status = OrderStatus.Cancelled;
		_cancellationReason = reason;
		_cancelledBy = cancelledBy;
		_modifiedDate = DateTime.UtcNow;

		if(cancelledBy == CancelledBy.Manager)
		{
			this.AddDomainEvent(new OrderCancelledDomainEvent(OrderNumber, previousStatus));
		}
	}

	public void SetStatus(OrderStatus newStatus)
	{
		if (!OrderStatus.CanTransition(Status, newStatus))
			throw new InvalidOperationException($"Cannot transition from {Status.Value} to {newStatus.Value}");

		Status = newStatus;
		_modifiedDate = DateTime.UtcNow;

		this.AddOrderStatusChangedDomainEvent(newStatus);
	}

	public void AddRequestedOrderDomainEvent()
	{
		this.AddDomainEvent(new OrderRequestedDomainEvent(this));
	}

	private void AddOrderStatusChangedDomainEvent(OrderStatus orderStatus)
	{
		IAggregateEvent domainEvent = orderStatus.Value switch
		{
			"Confirmed" => new OrderConfirmedDomainEvent(this),
			"InPreparation" => new OrderPreparationStartedDomainEvent(this.Id,this.OrderNumber),
			"AwaitingDispatch" => new OrderPreparationFinishedDomainEvent(this.Id, this.OrderNumber),
			"Approved" => new OrderApprovedDomainEvent(this.OrderNumber),
			_ => throw new InvalidOperationException($"No domain event defined for status {orderStatus.Value}")
		};

		this.AddDomainEvent(domainEvent);
	}

	public void AddPaymentConfirmedForThisOrderDomainEvent()
	{
		this.AddDomainEvent(new PaymentConfirmedDomainEvent(this.OrderDetails.ToList()));
	}

	public void SetInvoiceURL(string? installmentCode, string invoiceURL) 
	{
		if (string.IsNullOrEmpty(installmentCode))
		{
			this._paymentInfo.SetInvoiceUrl(installmentCode, invoiceURL); 
			return;
		}
			
		this._paymentInfo.SetInvoiceUrl(invoiceURL);
	}
}
