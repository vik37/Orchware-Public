using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Primitives;

namespace Orchware.Backoffice.Infrastructure.Persistence.Repository;

public class OrderRepository : IOrderRepository
{
	private readonly OrchwareBackofficeDbContext _context;

	public IUnitOfWork UnitOfWork { get; }

	public OrderRepository(OrchwareBackofficeDbContext context, IUnitOfWork unitOfWork)
	{
		_context = context;
		UnitOfWork = unitOfWork;
	}

	public async Task<IList<Order>> GetAllAsync()
	{
		return await _context.Order
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<Order?> GetByIdAsync(Guid id)
	{
		return await _context.Order
			.Include(o => o.PaymentInfo)
			.ThenInclude(p => p.Installments)
			.Include(o => o.OrderDetails)
			.AsSplitQuery()
			.FirstOrDefaultAsync(o => o.Id == id);
	}

	public async Task AddAsync(Order entity)
	{
		await _context.Order.AddAsync(entity);
	}

	public async Task DeleteAsync(Guid id)
	{
		var order = await _context.Order.FirstOrDefaultAsync(o => o.Id == id);
		if (order is not null)
		{
			_context.Order.Remove(order);
		}
	}

	public Task UpdateAsync(Order entity)
	{
		_context.Order.Update(entity);
		return Task.CompletedTask;
	}

	public async Task<Order?> GetByOrderNumber(string number, CancellationToken cancellationToken = default)
	{
		return await _context.Order.Include(o => o.PaymentInfo)
									.ThenInclude(p => p.Installments)
									.Include(o => o.OrderDetails)
									.AsSplitQuery()
									.FirstOrDefaultAsync(o => o.OrderNumber == number, cancellationToken);
	}
}
