using Orchware.Frontoffice.API.Common.Contracts;

namespace Orchware.Frontoffice.API.Domain.Events;

public record OrderRequestedDomainEvent(
	Guid UserId,
	int? Discount,
	string OrderNumber,
	decimal Amount,
	decimal TotalAmount,
	PaymentDomainDto Payment,
	IReadOnlyCollection<OrderDetailsDomainDto> OrderDetails
) : IDomainEvent
{
	public DateTime OrderedDate { get; } = DateTime.UtcNow;
}

public record OrderDetailsDomainDto(decimal Quantity, decimal PricePerUnit, string UnitOfMeasure, decimal TotalPrice, ProductDomainDto Product);

public record ProductDomainDto(string Name, string SeasonalFruits, decimal Price, string Units, string Image);

public record PaymentDomainDto(string PaymentNumber, bool WillBePaidInFull, DateTime PaymentDate,
	int? NumberOfInstallments, decimal? AmountPerInstallment, DateTime? FinalInstallmentDueDate, IReadOnlyCollection<InstallmentDomainDto>? Installment = null);

public record InstallmentDomainDto(string InstallmentCode, DateTime DueDate, decimal Amount);