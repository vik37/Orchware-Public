using Microsoft.EntityFrameworkCore;
using Orchware.Backoffice.Domain.Entities.Inventory;
using Orchware.Backoffice.Domain.Entities.Orders;
using Orchware.Backoffice.Domain.Entities.Shipping;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.InventoryConfigurations;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.ShipmentConfigurations;

namespace Orchware.Backoffice.Infrastructure.Persistence;

public class OrchwareBackofficeDbContext : DbContext
{
	public OrchwareBackofficeDbContext(DbContextOptions<OrchwareBackofficeDbContext> options)
		: base(options){}

	public DbSet<Product> Product { get; set; }
	public DbSet<Shelf> Shelf { get; set; }
	public DbSet<Order> Order { get; set; }
	public DbSet<Shipment> Shipment { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new ProductConfiguration());
		modelBuilder.ApplyConfiguration(new ShelfConfiguration());
		modelBuilder.ApplyConfiguration(new OrderConfiguration());
		modelBuilder.ApplyConfiguration(new OrderDetailsConfiguration());
		modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
	}
}
