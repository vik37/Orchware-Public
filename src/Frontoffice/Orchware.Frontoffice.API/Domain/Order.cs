using Utilities.Converters;

namespace Orchware.Frontoffice.API.Domain;

public class Order : BaseAuditableEntityGuid
{
	private readonly List<OrderDetails> _orderDetails = new();
	private OrderStatus _status = OrderStatus.Requested;

	public OrderStatus Status => _status;
	public int CompanyId { get; private set; }
	public Company Company { get; private set; }
	public Guid UserId { get; private set; }
	public User User { get; private set; }
	public DateTime? OrderDate { get; private set; }
	public int? Discount { get; private set; }
	public string OrderNumber { get; private set; }

	public decimal Amount => _orderDetails.Sum(d => d.PricePerUnit * (decimal)d.Quantity);
	public decimal TotalAmount => Discount.HasValue
		? Amount - (Amount * Discount.Value / 100m)
		: Amount;

	public IReadOnlyCollection<OrderDetails> OrderDetails => _orderDetails.AsReadOnly();

	#nullable disable
	public Order(){}
	#nullable enable

	public Order(User user, int? discount = null)
	{
		Company = user.Company ?? throw new ArgumentNullException(nameof(user.Company));
		User = user ?? throw new ArgumentNullException(nameof(user));
		CompanyId = user.CompanyId ?? throw new ArgumentNullException(nameof(user.CompanyId));
		UserId = user.Id;
		OrderDate = DateTime.UtcNow;
		Discount = discount;

		OrderNumber = $"#ON-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")}".Substring(0, 17);
	}

	public void AddOrderDetail(OrderDetails detail)
	{
		if (detail == null) throw new ArgumentNullException(nameof(detail));

		_orderDetails.Add(detail);
	}

	public void AddOrUpdateDetails(IReadOnlyCollection<OrderDetails> newDetails)
	{
		foreach (var newItem in newDetails)
		{
			var existing = this.OrderDetails.FirstOrDefault(x => x.ProductId == newItem.ProductId);
			if (existing is not null)
			{
				if(newItem.UnitOfMeasure == existing.UnitOfMeasure)
				{
					existing.Quantity += newItem.Quantity;
				}
				else
				{
					var convertedQuantity = UnitConverter.Convert(
						newItem.Quantity,
						newItem.UnitOfMeasure,
						existing.UnitOfMeasure
					);

					existing.Quantity += convertedQuantity;
				}
				
				existing.CalculateTotalPrice();
			}
			else
			{
				this.AddOrderDetail(newItem);
			}
		}
	}

	public void SetStatus(OrderStatus newStatus)
	{
		if (!OrderStatus.CanTransition(_status,newStatus))
			throw new InvalidOperationException($"Cannot transition from {_status.Value} to {newStatus.Value}");

		_status = newStatus;
	}
}

public class OrderStatus : IEquatable<OrderStatus>
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

	private static readonly Dictionary<OrderStatus, List<OrderStatus>> _transitions = new()
	{
		{ OrderStatus.Pending, new() { OrderStatus.Requested, OrderStatus.Cancelled } },
		{ OrderStatus.Requested, new() { OrderStatus.Confirmed, OrderStatus.Cancelled } },
		{ OrderStatus.Confirmed, new() { OrderStatus.InPreparation, OrderStatus.Cancelled } },
		{ OrderStatus.InPreparation, new() { OrderStatus.AwaitingDispatch, OrderStatus.Cancelled } },
		{ OrderStatus.AwaitingDispatch, new() { OrderStatus.Approved, OrderStatus.Cancelled } },
		{ OrderStatus.Approved, new() { } }, // Terminal state
        { OrderStatus.Cancelled, new() { } } // Terminal state
    };

	public static bool CanTransition(OrderStatus from, OrderStatus to)
		=> _transitions.TryGetValue(from, out var nextStates) && nextStates.Contains(to);

	public bool Equals(OrderStatus? other)
		=> other is not null && Value == other.Value;

	public override bool Equals(object? obj)
		=> Equals(obj as OrderStatus);

	public override int GetHashCode()
		=> Value.GetHashCode(StringComparison.Ordinal);

	public override string ToString() => Value;
}