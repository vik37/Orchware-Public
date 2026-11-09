using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;

public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
{
	public void Configure(EntityTypeBuilder<OrderDetails> builder)
	{
		builder.ToTable(nameof(OrderDetails), DomainSchema.Order);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.OrderId)
		   .HasColumnType("uniqueidentifier")
		   .HasColumnOrder(2)
		   .IsRequired();

		builder.Property<string>("_product")
		   .UsePropertyAccessMode(PropertyAccessMode.Field)
		   .HasColumnName("Product")
		   .HasColumnType("nvarchar(max)")
		   .HasColumnOrder(3)
		   .IsRequired();

		builder.Ignore(x => x.Product);

		builder.Property(x => x.Quantity)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("Quantity")
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.PricePerUnit)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("PricePerUnit")
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.UnitOfMeasure)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("UnitOfMeasure")
			.HasColumnType("nvarchar(50)")
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.TotalPrice)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("TotalPrice")
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(7)
			.IsRequired();

		builder.Property<string>("_shelfCode")
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("ShelfCode")
			.HasColumnType("nvarchar(20)")
			.HasColumnOrder(8)
			.IsRequired();

		builder.Property(x => x.ShelfProductId)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("ShelfProductId")
			.HasColumnOrder(9)
			.IsRequired();
	}
}
