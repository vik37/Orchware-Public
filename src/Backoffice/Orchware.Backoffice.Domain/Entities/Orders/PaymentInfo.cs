using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Orders;

public class PaymentInfo : Entity<int>
{
	public Guid OrderId { get; set; }

	private string _paymentNumber = string.Empty;
	public string PaymentNumber { get => _paymentNumber; }

	private bool _willBePaidInFull;
	public bool WillBePaidInFull { get => _willBePaidInFull; }

	private DateTime _paymentDate;
	public DateTime PaymentDate { get => _paymentDate; }

	private bool _isPaidInFull;
	public bool IsPaidInFull { get => _isPaidInFull; }

	private DateTime? _paidInFullDate;
	public DateTime? PaidInFullDate { get => _paidInFullDate; }

	private int? _numberOfInstallments;
	public int? NumberOfInstallments { get => _numberOfInstallments; }

	private decimal? _amountPerInstallment;
	public decimal? AmountPerInstallment { get => _amountPerInstallment; }

	private string? _invoiceUrl;
	public string? InvoiceUrl { get => _invoiceUrl; }

	private DateTime? _finalInstallmentDueDate;
	public DateTime? FinalInstallmentDueDate { get => _finalInstallmentDueDate; }

	private readonly List<Installment> _installments = new();
	public IReadOnlyCollection<Installment>? Installments => _installments?.AsReadOnly();

	public PaymentInfo()
	{
	}

	public PaymentInfo(string paymentNumber, bool willBePaidInFull, DateTime paymentDate,
		List<Installment>? installments, int? numberOfInstallments = null, decimal? amountPerInstallment = null,
		DateTime? finalInstallmentDueDate = null)
	{
		_paymentNumber = paymentNumber;
		_willBePaidInFull = willBePaidInFull;
		_paymentDate = paymentDate;
		_isPaidInFull = false;
		_paidInFullDate = null;
		_numberOfInstallments = numberOfInstallments;
		_amountPerInstallment = amountPerInstallment;
		_finalInstallmentDueDate = finalInstallmentDueDate;

		if (!WillBePaidInFull && (installments is null || !installments.Any()))
			throw new InvalidOperationException("Installments required when not paid in full.");

		if (installments != null)
		{
			_installments.AddRange(installments);
		}
	}

	private void FullyPaidInstallments()
	{
		if (Installments!.All(i => i.IsPaid))
		{
			this._isPaidInFull = true;
			this._paidInFullDate = DateTime.UtcNow;
		}
	}

	public void MarkInstallmentAsPaid(string installmentCode)
	{
		if(this.Installments != null && this.Installments.Any())
		{
			var installment = this.Installments.FirstOrDefault(i => i.InstallmentCode == installmentCode);

			if (installment is not null)
			{
				installment.MarkAsPaid(DateTime.UtcNow);
				this.FullyPaidInstallments();
			}
		}
	}

	public void MarkAsPaid()
	{
		if (IsPaidInFull) return;

		this._isPaidInFull = true;
		this._paidInFullDate = DateTime.UtcNow;
	}

	public void SetInvoiceUrl(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
			throw new ArgumentException("URL cannot be empty.", nameof(url));
			
		_invoiceUrl = url;
	}

	public void SetInvoiceUrl(string installmentCode, string url)
	{
		var installment = _installments.FirstOrDefault(i => i.InstallmentCode == installmentCode);

		if (installment is null)
			throw new InvalidOperationException($"Installment with ID {installmentCode} not found.");

		installment.SetInvoiceUrl(url);
	}
}
