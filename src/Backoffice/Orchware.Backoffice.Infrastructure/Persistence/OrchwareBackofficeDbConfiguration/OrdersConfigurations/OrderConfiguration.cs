using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> builder)
	{
		builder.ToTable(nameof(Order), DomainSchema.Order, o => o.HasTrigger($"[{DomainSchema.Order}].[TR_Orders_UpdateProcessingLevel]"));

		builder.Property(x => x.Id)
		   .HasColumnType("uniqueidentifier")
		   .HasDefaultValueSql("NEWSEQUENTIALID()");

		builder.Property(x => x.OrderNumber)
						.HasColumnName("OrderNumber")
						.HasColumnType("nvarchar(20)")
						.IsRequired()
						.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.Property(x => x.Status)
			.HasColumnType("nvarchar(100)")
			.HasConversion(
				x => x.Value.ToString(),
				x => OrderStatus.GetAllStatuses().FirstOrDefault(s => s.Value == x) ?? OrderStatus.Requested
			)
			.IsRequired();

		builder.Property(o => o.VisibilityLevel)
				.HasConversion(
					p => p.Value,
					v => VisibilityLevel.FromString(v))
				.HasColumnName("VisibilityLevel")
				.IsRequired();

		builder.Property<DateTime>(x => x.OrderedDate)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("OrderedDate")
			.IsRequired();

		builder.OwnsOne(x => x.Buyer, b =>
		{
			b.WithOwner();

			b.Property(x => x.Name)
				.HasColumnType("nvarchar(550)")
				.IsRequired();

			b.Property(x => x.JobTitle)
				.HasColumnType("nvarchar(850)")
				.IsRequired();

			b.Property(x => x.PersonalEmail)
				.HasColumnType("nvarchar(400)")
				.IsRequired();

			b.Property(x => x.CompanyName)
				.HasColumnType("nvarchar(550)")
				.IsRequired();

			b.Property(x => x.CompanyCity)
				.HasColumnType("nvarchar(100)")
				.IsRequired();

			b.Property(x => x.CompanyLocation)
				.HasColumnType("nvarchar(250)")
				.IsRequired();

			b.Property(x => x.CompanyEmail)
				.HasColumnType("nvarchar(400)")
				.IsRequired();

			b.Property(x => x.CompanyAddress)
				.HasColumnType("nvarchar(max)")
				.IsRequired();

			b.Property(x => x.CompanyPhoneNumber)
				.HasColumnType("nvarchar(30)")
				.IsRequired();

			b.Property(x => x.CompanyRegisteredDate)
					.IsRequired();
		});

		builder.Property(x => x.Amount)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("Amount")
			.HasColumnType("decimal(18,2)")
			.IsRequired();

		builder.Property(x => x.TotalAmount)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("TotalAmount")
			.HasColumnType("decimal(18,2)")
			.IsRequired();

		builder.Property(x => x.Discount)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("Discount");

		builder.Property(x => x.CancellationReason)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("CancellationReason")
			.HasColumnType("nvarchar(max)")
			.IsRequired(false);

		builder.Property(x => x.CancelledBy)
			.HasConversion<int>()
			.HasColumnName("CancelledBy")
			.HasField("_cancelledBy")
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.Property(x => x.CreatedDate)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("CreatedDate")
			.IsRequired();

		builder.Property(x => x.ModifiedDate)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("ModifiedDate");

		builder.HasMany(x => x.OrderDetails)
			.WithOne()
			.HasForeignKey(x => x.OrderId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.PaymentInfo)
			.WithOne()
			.HasForeignKey<PaymentInfo>(x => x.OrderId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(x => x.OrderNumber)
			   .HasDatabaseName("IX_Order_OrderNumber")
			   .IsUnique();

		builder.HasIndex(x => x.Status)
			.HasDatabaseName("IX_Order_Status");

		builder.HasIndex(x => x.VisibilityLevel)
			.HasDatabaseName("IX_Order_VisibilityLevel");

		builder.Metadata.FindNavigation(nameof(Order.OrderDetails))!.SetPropertyAccessMode(PropertyAccessMode.Field);
		builder.Metadata.FindNavigation(nameof(Order.PaymentInfo))!.SetPropertyAccessMode(PropertyAccessMode.Field);
	}
}
