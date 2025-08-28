using Microsoft.EntityFrameworkCore;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence
{
	public class OrchwareDbContext : DbContext
	{
		public OrchwareDbContext(DbContextOptions<OrchwareDbContext> options)
			: base(options)
		{}

		public DbSet<Company> Company { get; set; }
		public DbSet<Product> Product { get; set; }
		public DbSet<Payment> Payment { get; set; }
		public DbSet<Order> Order { get; set; }
		public DbSet<User> User { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new CompanyEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new OrderDetailsEntityTypeConfigurations());
			modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
			modelBuilder.ApplyConfiguration(new  UserEntityTypeConfiguration());
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			foreach (var entry in ChangeTracker.Entries<IAuditable>()
			.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
				{
					entry.Entity.ModifiedDate = DateTime.UtcNow;

					if (entry.State == EntityState.Added)
					{
						entry.Entity.CreatedDate = DateTime.UtcNow;
					}
				}

				return base.SaveChangesAsync(cancellationToken);
		}
	}
}
