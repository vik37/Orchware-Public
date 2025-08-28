using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
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

		builder.Property(x => x.UserId)
			.HasColumnType("uniqueidentifier")
			.HasColumnOrder(9);

		builder.Property(x => x.CreatedDate)
			.HasColumnOrder(10);

		builder.Property(x => x.ModifiedDate)
			.HasColumnOrder(11);

		builder.HasMany(x => x.OrderDetails)
			.WithOne(x => x.Order)
			.HasForeignKey(x => x.OrderId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.User)
			.WithMany(x => x.Orders)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
