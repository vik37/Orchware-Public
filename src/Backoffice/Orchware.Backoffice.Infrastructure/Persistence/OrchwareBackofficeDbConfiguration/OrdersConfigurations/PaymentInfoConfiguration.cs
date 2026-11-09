using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;

public class PaymentInfoConfiguration : IEntityTypeConfiguration<PaymentInfo>
{
	public void Configure(EntityTypeBuilder<PaymentInfo> builder)
	{
		builder.ToTable(nameof(PaymentInfo), DomainSchema.Order);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.OrderId)
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.PaymentNumber)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("PaymentNumber")
			.HasColumnType("nvarchar(20)")
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.WillBePaidInFull)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("WillBePaidInFull")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.PaymentDate)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("PaymentDate")
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.IsPaidInFull)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("IsPaidInFull")
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.PaidInFullDate)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("PaidInFullDate")
			.HasColumnOrder(7);

		builder.Property(x => x.NumberOfInstallments)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("NumberOfInstallments")
			.HasColumnOrder(8);

		builder.Property(x => x.AmountPerInstallment)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("AmountPerInstallment")
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(9);

		builder.Property(x => x.FinalInstallmentDueDate)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("FinalInstallmentDueDate")
			.HasColumnOrder(10);

		builder.Property(x => x.InvoiceUrl)
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("InvoiceUrl")
			.HasColumnType("nvarchar(2048)")
			.HasColumnOrder(11)
			.IsRequired(false);

		builder.HasMany(x => x.Installments)
			.WithOne()
			.HasForeignKey(x => x.PaymentInfoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Metadata.FindNavigation(nameof(PaymentInfo.Installments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
	}
}
