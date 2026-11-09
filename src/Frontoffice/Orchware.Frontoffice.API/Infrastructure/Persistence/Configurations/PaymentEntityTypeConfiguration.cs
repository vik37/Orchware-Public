using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.UserId)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.OrderId)
		   .HasColumnType("uniqueidentifier")
		   .HasColumnOrder(3)
		   .IsRequired();

		builder.Property<string>("_paymentNumber")
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("PaymentNumber")
			.HasColumnType("nvarchar(20)")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.PaymentDate)
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.WillBePaidInFull)
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.IsPaidInFull)
			.HasColumnOrder(7)
			.IsRequired();

		builder.Property(x => x.PaidInFullDate)
			.HasColumnOrder(8);

		builder.Property(x => x.NumberOfInstallments)
			.HasColumnOrder(9);

		builder.Property(x => x.AmountPerInstallment)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(10);

		builder.Property(x => x.FinalInstallmentDueDate)
			.HasColumnOrder(11);

		builder.Property(x => x.PaymentRuleId)
			.HasColumnOrder(12);

		builder.Property(x => x.CreatedDate)
			.HasColumnOrder(13)
			.IsRequired();

		builder.Property(x => x.ModifiedDate)
			.HasColumnOrder(14);

		builder.Property(x => x.InvoiceUrl)
			.HasColumnName("InvoiceUrl")
			.HasColumnType("nvarchar(2048)")
			.HasColumnOrder(15)
			.IsRequired(false);

		builder.HasIndex(x => new {x.OrderId, x.UserId}).IsUnique();

		builder.HasOne(x => x.Rules)
			.WithMany()
			.HasForeignKey(x => x.PaymentRuleId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasMany(x => x.Installments)
			.WithOne(x => x.Payment)
			.HasForeignKey(x => x.PaymentId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
