using System.Text.Json.Serialization;

namespace Orchware.Frontoffice.API.Infrastructure.Cache.Models;

public class RequestedOrderCacheModel
{
	public UserCacheDto User { get; }
	public int? Discount { get; }
	public string OrderNumber { get;}
	public decimal Amount { get;  }
	public decimal TotalAmount { get; }
	public PaymentCacheDto Payment { get; }
	public DateTime OrderedDate { get; }
	public IReadOnlyCollection<OrderDetailsCacheDto> OrderDetails { get; }

	[JsonConstructor]
	public RequestedOrderCacheModel(
			UserCacheDto user,
			int? discount,
			string orderNumber,
			decimal amount,
			decimal totalAmount,
			PaymentCacheDto payment,
			DateTime orderedDate,
			IReadOnlyCollection<OrderDetailsCacheDto> orderDetails
		)
	{
		User = user;
		Discount = discount;
		OrderNumber = orderNumber;
		Amount = amount;
		TotalAmount = totalAmount;
		Payment = payment;
		OrderedDate = orderedDate;
		OrderDetails = orderDetails;
	}
}

public class OrderDetailsCacheDto
{
	public decimal Quantity { get; }
	public decimal PricePerUnit { get;}
	public string UnitOfMeasure { get; }
	public decimal TotalPrice { get; }
	public ProductCacheDto Product { get; }

	[JsonConstructor]
	public OrderDetailsCacheDto(decimal quantity, decimal pricePerUnit, string unitOfMeasure, decimal totalPrice,
	ProductCacheDto product)
	{
		Quantity = quantity;
		PricePerUnit = pricePerUnit;
		UnitOfMeasure = unitOfMeasure;
		TotalPrice = totalPrice;
		Product = product;
	}
}

public class ProductCacheDto
{
	public string Name { get; }
	public string SeasonalFruits { get; }
	public decimal Price { get; }
	public string Units { get; }
	public string Image { get; }

	[JsonConstructor]
	public ProductCacheDto(string name, string seasonalFruits, decimal price, string units, string image)
	{
		Name = name;
		SeasonalFruits = seasonalFruits;
		Price = price;
		Units = units;
		Image = image;
	}
};

public class UserCacheDto
{
	public string Name { get; }
	public string JobTitle { get; }
	public string PersonalEmail { get; }
	public string CompanyName { get; }
	public string CompanyEmail { get; }
	public string CompanyAddress { get; }
	public string CompanyCity { get; }
	public string CompanyLocation { get; }
	public string CompanyPhoneNumber { get; }
	public DateTime CompanyRegisteredDate { get; }

	[JsonConstructor]
	public UserCacheDto(string name, string jobTitle, string personalEmail, string companyName, string companyEmail,
			string companyAddress, string companyCity, string companyLocation, string companyPhoneNumber, DateTime companyRegisteredDate)
	{
		Name = name;
		JobTitle = jobTitle;
		PersonalEmail = personalEmail;
		CompanyName = companyName;
		CompanyEmail = companyEmail;
		CompanyAddress = companyAddress;
		CompanyCity = companyCity;
		CompanyLocation = companyLocation;
		CompanyPhoneNumber = companyPhoneNumber;
		CompanyRegisteredDate = companyRegisteredDate;
	}
}

public class PaymentCacheDto
{
	public string PaymentNumber { get; }
	public bool WillBePaidInFull { get; }
	public DateTime PaymentDate { get; }
	public int? NumberOfInstallments { get; }
	public decimal? AmountPerInstallment { get; }
	public DateTime? FinalInstallmentDueDate { get; }
	public IReadOnlyCollection<InstallmentCacheDto>? Installments { get; }

	[JsonConstructor]
	public PaymentCacheDto(string paymentNumber, bool willBePaidInFull, DateTime paymentDate,
			int? numberOfInstallments, decimal? amountPerInstallment, DateTime? finalInstallmentDueDate,
			IReadOnlyCollection<InstallmentCacheDto>? installments)
	{
		PaymentNumber = paymentNumber;
		WillBePaidInFull = willBePaidInFull;
		PaymentDate = paymentDate;
		NumberOfInstallments = numberOfInstallments;
		AmountPerInstallment = amountPerInstallment;
		FinalInstallmentDueDate = finalInstallmentDueDate;
		Installments = installments;
	}
}

public class InstallmentCacheDto
{
	public string InstallmentCode { get; }
	public DateTime DueDate { get; }
	public decimal Amount { get; }

	[JsonConstructor]
	public InstallmentCacheDto(string installmentCode, DateTime dueDate, decimal amount)
	{
		InstallmentCode = installmentCode;
		DueDate = dueDate;
		Amount = amount;
	}
}