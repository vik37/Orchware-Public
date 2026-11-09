using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Proccessing;
using Orchware.Backoffice.Domain.Enums;

namespace Orchware.Backoffice.Infrastructure.Persistence.Repository;

public class BilingTaskRepository : IBilingTaskRepository
{
	private readonly OrchwareBackofficeDbContext _context;

	public BilingTaskRepository(OrchwareBackofficeDbContext context)
	{
		_context = context;
	}

	public async Task<List<BillingTask>> GetForProccessing()
	{
		var billingTask = await _context.BilingTask
			.Where(x => (x.RetryCount <= 3 && x.Status == ProcessStatus.Failed) || (x.Status == ProcessStatus.Pending || (x.Status == ProcessStatus.InProgress && !x.IsFullPayment && x.InstalmentNumber.HasValue && 
							x.NumberOfInstallments.HasValue && x.InstalmentNumber <= x.NumberOfInstallments && (x.ScheduledFor.Value > DateTime.UtcNow.AddMinutes(-5) 
							&& x.ScheduledFor.Value < DateTime.UtcNow.AddMinutes(5))))).OrderByDescending(x => x.CreatedDate).Take(10).ToListAsync();

		return billingTask;
	}

	public async Task AddBillingTask(BillingTask billingTask, bool includeSaveChanges = true)
	{
		await _context.BilingTask.AddAsync(billingTask);
		if (includeSaveChanges)
			await _context.SaveChangesAsync();
	}

	public async Task UpdateBulkBillingTask(List<BillingTask> billingTasks)
	{
		if (billingTasks == null || billingTasks.Count == 0)
			return;

		foreach (var task in billingTasks)
		{
			_context.BilingTask.Attach(task);
			_context.Entry(task).State = EntityState.Modified;
		}

		await _context.SaveChangesAsync();
	}

	public async Task<bool> CheckIfOrderAllreadyRegistered(Guid orderId)
	{
		return await _context.BilingTask.AsNoTracking().AnyAsync(x => x.OrderId == orderId);
	}
}
