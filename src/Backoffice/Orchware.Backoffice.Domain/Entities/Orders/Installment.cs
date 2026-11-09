using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public class Installment : Entity<int>
{
	public DateTime DueDate { get; private set; }
	public decimal Amount { get; private set; }
	public bool IsPaid { get; private set; } = false;
	public DateTime? PaidOnDate { get; private set; }
	public string InstallmentCode { get; private set; } = string.Empty;
	public int PaymentInfoId { get; private set; }
	public string? InvoiceUrl { get; set; }

	public Installment(){}

	public Installment(DateTime dueDate, decimal amount, string installmentCode)
	{
		DueDate = dueDate;
		Amount = amount;
		InstallmentCode = installmentCode;
	}

	public void MarkAsPaid(DateTime paidOn)
	{
		if (IsPaid) return;

		IsPaid = true;
		PaidOnDate = paidOn;
	}

	public void SetInvoiceUrl(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
			throw new ArgumentException("URL cannot be empty.", nameof(url));

		InvoiceUrl = url;
	}
}