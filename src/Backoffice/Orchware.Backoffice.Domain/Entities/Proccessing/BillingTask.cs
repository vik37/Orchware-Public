using Orchware.Backoffice.Domain.Enums;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Domain.Entities.Proccessing;

public class BillingTask : Entity<int>
{
	#region Main Order
	public Guid OrderId { get; private set; }
	public ProcessStatus Status { get; private set; }
	public bool IsFullPayment { get; private set; } = true;
	#endregion

	#region Possible Installments
	public int? InstallmentId { get; private set; }
	public int? InstalmentNumber { get; private set; }
	public int? NumberOfInstallments { get; private set; }
	public DateTime? ScheduledFor { get; private set; }
	#endregion

	public int RetryCount { get; private set; } = 0;
	public string? LastErrorMessage { get; private set; }
	public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
	public DateTime? ProcessedAt { get; private set; }

	private BillingTask() { }

	public BillingTask(Guid orderId, bool isFullPayment, 
		int? installmentId = null, int? numberOfInstallments = null)
	{
		OrderId = orderId;
		Status = ProcessStatus.Pending;
		IsFullPayment = isFullPayment;
		CreatedDate = DateTime.Now;
		InstalmentNumber = 1;

		if (!isFullPayment)
		{
			ScheduledFor = DateTime.UtcNow.AddMonths(1);
			InstallmentId = !isFullPayment ? installmentId : null;
			NumberOfInstallments = !isFullPayment ? numberOfInstallments : null;
			InstalmentNumber = 1;
		}
	}

	public void MarkAsProcessing() => Status = ProcessStatus.InProgress;

	public void SetInstallmentId(int? installmentId) => InstallmentId = installmentId;

	public bool SetInstallmentNumber()
	{
		InstalmentNumber++;
		if(InstalmentNumber <= NumberOfInstallments)
		{
			ScheduledFor = DateTime.UtcNow.AddMonths(1);
			MarkAsProcessing();

			return false;
		}

		MarkAsCompleted();
		return true;
	}

	public void MarkAsCompleted()
	{
		Status = ProcessStatus.Completed;
		ProcessedAt = DateTime.UtcNow;
		LastErrorMessage = null;
	}

	public void MarkAsFailed(string error)
	{
		Status = ProcessStatus.Failed;
		LastErrorMessage = error;
		RetryCount++;
	}

	public bool CanRetry() => RetryCount < 3;
}
