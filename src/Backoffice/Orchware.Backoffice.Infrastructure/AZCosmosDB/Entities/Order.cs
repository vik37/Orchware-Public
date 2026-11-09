using Newtonsoft.Json;

namespace Orchware.Backoffice.Infrastructure.AZCosmosDB.Entities;

public class Order
{
	[JsonProperty("id")]
	public string Id { get; set; } = Guid.NewGuid().ToString();

	[JsonProperty("orderNumber")]
	public string OrderNumber { get; set; } = string.Empty;

	[JsonProperty("orderedDate")]
	public DateTime OrderedDate { get; set; }

	public Buyer Buyer { get; set; } = new();

	[JsonProperty("discount")]
	public int? Discount { get; set; }

	public PaymentInfo PaymentInfo { get; set; } = new PaymentInfo();

	[JsonProperty("createdDate")]
	public DateTime CreatedDate { get; set; }

	public List<OrderDetails> OrderDetails { get; set; } = new();

	[JsonProperty("amount")]
	public decimal Amount { get; set; }

	[JsonProperty("totalAmount")]
	public decimal TotalAmount { get; set; }

	[JsonProperty("url")]
	public string? URL { get; set; }

	[JsonProperty("ttl", NullValueHandling = NullValueHandling.Ignore)]
	public int? Ttl { get; set; }

	[JsonProperty("environment")]
	public string Environment { get; set; } = "Development";

	[JsonProperty("invoice")]
	public InvoiceInfo Invoice { get; set; } = new InvoiceInfo()
	{
		PdfGenerated = false
	};
}

public class Buyer
{
	public string Name { get; set; } = string.Empty;
	public string JobTitle { get; set; } = string.Empty;
	public string PersonalEmail { get; set; } = string.Empty;
	public string CompanyName { get; set; } = string.Empty;
	public string CompanyEmail { get; set; } = string.Empty;
	public string CompanyAddress { get; set; } = string.Empty;
	public string CompanyCity { get; set; } = string.Empty;
	public string CompanyLocation { get; set; } = string.Empty;
	public string CompanyPhoneNumber { get; set; } = string.Empty;
}

public sealed class OrderDetails
{
	public OrderedProduct Product { get; set; } = new();

	public decimal Quantity { get; set; }

	public decimal PricePerUnit { get; set; }

	public string UnitOfMeasure { get; set; } = string.Empty;

	public decimal TotalPrice { get; set; }
}

public class OrderedProduct
{
	public string Name { get; set; } = string.Empty;
	public string SeasonalFruits { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public string Units { get; set; } = string.Empty;
}

public class PaymentInfo
{
	public string PaymentNumber { get; set; } = string.Empty;

	public bool WillBePaidInFull { get; set; }

	public DateTime PaymentDate { get; set; }

	public int? NumberOfInstallments { get; set; }

	public decimal? AmountPerInstallment { get; set; }

	public DateTime? FinalInstallmentDueDate { get; set; }

	public Installment? Installments { get; set; }
}

public class Installment
{
	public int InstallmentNumber { get; set; }
	public DateTime DueDate { get; set; }
	public decimal Amount { get; set; }
	public string InstallmentCode {  get; set; } = string.Empty;
}

public class InvoiceInfo
{
	public bool PdfGenerated { get; set; }
	public string? PdfUrl { get; set; }
	public DateTime? GeneratedAt { get; set; }
}