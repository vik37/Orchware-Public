namespace Orchware.Frontoffice.API.Domain;

public class Payment : Base
{
	public string CustomerId { get; set; } = string.Empty;
	public Guid OrderId { get; set; }
	public DateTime PaymentDate { get; set; }
	public bool PayImmediately { get; set; } = true;
	public DateTime? SelectedPaymentDate { get; set; }
	public int MaxsimumAllowedNumberOfMonths { get; set; }
}
