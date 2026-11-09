using Orchware.Frontoffice.API.Common.CustomExceptions;
using Utilities.Converters;

namespace Orchware.Frontoffice.API.Domain;

public class Payment : BaseAuditableEntityInt
{
	public string UserId { get; set; } = string.Empty;
	public Guid OrderId { get; set; }
	private string _paymentNumber;
	public string PaymentNumber { get { return _paymentNumber; } }

	public bool WillBePaidInFull { get; set; } = true;

	/// <summary>
	/// Selected the day by which he could pay. 
	/// If paying in installments, then when can he/she start with the first installment? 
	/// He/She cannot exceed the MaximumPaymentPeriod set by the manager, until which maximum number of days he can choose.
	/// </summary>
	public DateTime PaymentDate { get; set; }

	public bool IsPaidInFull { get; set; } = false;
	// Date when the full amount was paid
	public DateTime? PaidInFullDate { get; set; }

	public int? NumberOfInstallments { get; set; }
	public decimal? AmountPerInstallment { get; set; }

	// Final payment date for the last installment
	public DateTime? FinalInstallmentDueDate { get; private set; }

	public string? InvoiceUrl { get; private set; }

	public int PaymentRuleId { get; private set; }

	private PaymentRules _rules;

	public PaymentRules Rules { get { return _rules; } }

	public ICollection<Installment> Installments { get; private set; } = new List<Installment>();

	#nullable disable
	public Payment(){}
	#nullable enable

	public Payment(PaymentRules rules)
	{
		if(rules is null) throw new ArgumentNullException(nameof(rules));
			
		_rules = rules;
		PaymentRuleId = rules.Id;

		_paymentNumber = $"PAY-{DateTime.UtcNow.Year}-{CustomNumberGenerator.FiveDigitGenerator()}";
	}

	public void SetDetailsAndGenerateInstallments(decimal totalOrderAmount, bool willBePaidInFull, int? numberOfInstallments, 
		decimal? amountPerInstallment, DateTime paymentDate)
	{
		this.WillBePaidInFull = willBePaidInFull;
		this.NumberOfInstallments = numberOfInstallments;
		this.AmountPerInstallment = amountPerInstallment;
		this.PaymentDate = paymentDate;

		if (!WillBePaidInFull)
		{
			if (this.NumberOfInstallments > this.Rules.MaxAllowedInstallments)
				throw new ArgumentException("Invalid Number of Installments");

			TimeSpan maxPaymentChosenDate = Rules.MaximumPaymentPeriod + Rules.GracePeriod;

			if (this.PaymentDate > DateTime.UtcNow.Add(maxPaymentChosenDate))
				throw new BadRequestException($"Payment date cannot be more than {maxPaymentChosenDate.TotalDays} days in the future.");

			this.AmountPerInstallment = Math.Round(totalOrderAmount / NumberOfInstallments!.Value, 2);
			GenerateInstallments(totalOrderAmount);
		}
	}

	private void GenerateInstallments(decimal totalOrderAmount)
	{
		if (this.NumberOfInstallments is null or <= 0) return;

		Installment.ResetNumbering();
		decimal accumulatedAmount = 0;

		for (int i = 0; i < this.NumberOfInstallments; i++)
		{
			decimal currentInstallmentAmount;

			if (i == this.NumberOfInstallments - 1)
			{
				currentInstallmentAmount = totalOrderAmount - accumulatedAmount;
			}
			else
			{
				currentInstallmentAmount = this.AmountPerInstallment.GetValueOrDefault();
				accumulatedAmount += currentInstallmentAmount;
			}

			var installment = new Installment(
				this,
				currentInstallmentAmount,
				this.PaymentDate.AddMonths(i + 1)
			);

			this.Installments.Add(installment);
		}

		this.FinalInstallmentDueDate = this.PaymentDate.AddMonths(NumberOfInstallments.Value - 1);
		Installment.ResetNumbering();
	}

	private void FullyPaidInstallments()
	{
		if (Installments.All(i => i.IsPaid))
		{
			IsPaidInFull = true;
			PaidInFullDate = DateTime.UtcNow;
		}
	}

	public void MarkInstallmentAsPaid(string installmentCode)
	{
		var installment = this.Installments.FirstOrDefault(i => i.InstallmentCode == installmentCode);

		if (installment != null)
		{
			installment.MarkAsPaid(DateTime.UtcNow);
			this.FullyPaidInstallments();
		}
	}

	public void MarkAsPaid(DateTime paidOn)
	{
		if (IsPaidInFull) return;

		IsPaidInFull = true;
		PaidInFullDate = paidOn;
	}

	public void SetInvoiceUrl(string url)
	{
		if (string.IsNullOrWhiteSpace(url))
			throw new ArgumentException("URL cannot be empty.", nameof(url));

		InvoiceUrl = url;
	}

	public void SetInvoiceUrl(string installmentCode, string url)
	{
		var installment = Installments.FirstOrDefault(i => i.InstallmentCode == installmentCode);

		if (installment is null)
			throw new InvalidOperationException($"Installment with ID {installmentCode} not found.");

		installment.SetInvoiceUrl(url);
	}
}
