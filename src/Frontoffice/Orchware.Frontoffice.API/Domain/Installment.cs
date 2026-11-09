namespace Orchware.Frontoffice.API.Domain;

public class Installment : BaseInt
{
	private static int _nextInstallmentNumber = 1;

	public int PaymentId { get; private set; }
	public Payment Payment { get; private set; } = null!;

	public int InstallmentNumber { get; private set; }
	public DateTime DueDate { get; private set; }
	public decimal Amount { get; private set; }
	public bool IsPaid { get; set; } = false;

	public string? InvoiceUrl { get; set; }

	// Date when the installment was paid
	public DateTime? PaidOnDate { get; set; }

	private string _installmentCode;
	public string InstallmentCode { get {  return _installmentCode; } }

	#nullable disable
	private Installment() { }
	#nullable enable

	public Installment(Payment payment, decimal amount, DateTime dueDate)
	{
		this.Payment = payment;
		this.Amount = amount;
		this.DueDate = dueDate;
		this.InstallmentNumber = _nextInstallmentNumber;
		_nextInstallmentNumber++;
		_installmentCode = $"{Payment.PaymentNumber}-{InstallmentNumber:D2}";
	}

	public static void ResetNumbering()
	{
		_nextInstallmentNumber = 1;
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
