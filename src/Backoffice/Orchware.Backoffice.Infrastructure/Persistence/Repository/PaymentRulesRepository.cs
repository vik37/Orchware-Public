
using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Settings;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Infrastructure.Persistence.Repository;

public class PaymentRulesRepository : IPaymentRulesRepository
{
	private readonly OrchwareBackofficeDbContext _context;

	public IUnitOfWork UnitOfWork { get; }

	public PaymentRulesRepository(OrchwareBackofficeDbContext context, IUnitOfWork unitOfWork)
	{
		_context = context;
		UnitOfWork = unitOfWork;
	}

	public async Task<IList<PaymentRules>> GetAllAsync() => await _context.PaymentRules.ToListAsync();

	public async Task<PaymentRules?> GetByIdAsync(int id) => await _context.PaymentRules.FirstOrDefaultAsync(x => x.Id == id);

	public async Task AddAsync(PaymentRules entity)
	{
		await _context.PaymentRules.AddAsync(entity);
	}

	public async Task DeleteAsync(int id)
	{
		var entity = await _context.PaymentRules.FirstOrDefaultAsync(entity => entity.Id == id);

		if (entity is not null)
		{
			_context.PaymentRules.Remove(entity);
		}
	}

	public Task UpdateAsync(PaymentRules entity)
	{
		_context.PaymentRules.Update(entity);
		return Task.CompletedTask;
	}
}
