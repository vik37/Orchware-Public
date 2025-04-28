using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.ToTable(nameof(Order), OrchwareBackofficeDbContext.OrderSchema);

		builder.Property(x => x.Id)
		   .HasColumnType("uniqueidentifier")
		   .HasDefaultValueSql("NEWSEQUENTIALID()")
		   .HasColumnOrder(1);

		builder.Property(x => x.Status)
			.HasColumnType("nvarchar(100)")
			.HasConversion(
				x => x.Value.ToString(),
				x => OrderStatus.GetAllStatuses().FirstOrDefault(s => s.Value == x) ?? OrderStatus.Pending
			)
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.CompanyId)
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.OrderDate)
			.HasColumnOrder(4);

		builder.Property(x => x.Amount)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.TotalAmount)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.Discount)
			.HasColumnOrder(7);

		builder.Property(x => x.OrderNumber)
			.HasColumnType("nvarchar(100)")
			.HasColumnOrder(8)
			.IsRequired();

		builder.Property(x => x.CreatedDate)
			.HasColumnOrder(9)
			.IsRequired();

		builder.Property(x => x.ModifiedDate)
			.HasColumnOrder(10);

		builder.Property(x => x.Payment)
			.HasColumnType("nvarchar(max)")
			.HasColumnOrder(11)
			.IsRequired();

		builder.HasMany(x => x.OrderDetails)
			.WithOne(x => x.Order)
			.HasForeignKey(x => x.OrderId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.OwnsOne(x => x.Company, c =>
		{
			c.WithOwner();
		});

		builder.Ignore(x => x.PaymentDetails);

		builder.Metadata.FindNavigation(nameof(Order.OrderDetails))!.SetPropertyAccessMode(PropertyAccessMode.Field);
	}
}
