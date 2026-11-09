using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Settings;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Entities.Shipping;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.InventoryConfigurations;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.SettingsCongiguration;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.ShipmentConfigurations;
using MediatR;
using Orchware.Backoffice.Infrastructure.Persistence.Extensions;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.ProcessingConfigurations;
using Orchware.Backoffice.Domain.Entities.Proccessing;

namespace Orchware.Backoffice.Infrastructure.Persistence;

public class OrchwareBackofficeDbContext : DbContext
{
	private readonly IMediator? _mediator;
	public OrchwareBackofficeDbContext(DbContextOptions<OrchwareBackofficeDbContext> options, IMediator mediator)
		: base(options)
	{
		_mediator = mediator;
	}

	public OrchwareBackofficeDbContext(DbContextOptions<OrchwareBackofficeDbContext> options)
		: base(options)
	{}

	public DbSet<Product> Product { get; set; }
	public DbSet<Shelf> Shelf { get; set; }
	public DbSet<Order> Order { get; set; }
	public DbSet<Shipment> Shipment { get; set; }
	public DbSet<PaymentRules> PaymentRules { get; set; }
	public DbSet<BillingTask> BilingTask { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new ProductConfiguration());
		modelBuilder.ApplyConfiguration(new ShelfConfiguration());
		modelBuilder.ApplyConfiguration(new OrderConfiguration());
		modelBuilder.ApplyConfiguration(new OrderDetailsConfiguration());
		modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
		modelBuilder.ApplyConfiguration(new PaymentRulesyConfiguration());
		modelBuilder.ApplyConfiguration(new PaymentInfoConfiguration());
		modelBuilder.ApplyConfiguration(new InstallmentConfiguration());
		modelBuilder.ApplyConfiguration(new BillingTaskConfiguration());
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		await _mediator.DispatchDomainEventsAsync(this, cancellationToken);
		return await base.SaveChangesAsync(cancellationToken);
	}
}
