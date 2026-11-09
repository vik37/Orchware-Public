namespace Orchware.Backoffice.Domain.Entities.Proccessing;

public interface IBilingTaskRepository
{
	Task<List<BillingTask>> GetForProccessing();
	Task<bool> CheckIfOrderAllreadyRegistered(Guid orderId);
	Task AddBillingTask(BillingTask billingTask, bool includeSaveChanges = true);
	Task UpdateBulkBillingTask(List<BillingTask> billingTasks);
}
