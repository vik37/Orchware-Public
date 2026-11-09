using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class InstallmentEntityTypeConfiguration : IEntityTypeConfiguration<Installment>
{
	public void Configure(EntityTypeBuilder<Installment> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.PaymentId)
			.HasColumnOrder(2)
			.IsRequired();

		builder.Ignore(x => x.InstallmentNumber);

		builder.Property(x => x.DueDate)
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.Amount)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.IsPaid)
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property<string>("_installmentCode")
			.UsePropertyAccessMode(PropertyAccessMode.Field)
			.HasColumnName("InstallmentCode")
			.HasColumnType("nvarchar(25)")
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.InvoiceUrl)
			.HasColumnName("InvoiceUrl")
			.HasColumnType("nvarchar(2048)")
			.HasColumnOrder(7)
			.IsRequired(false);
	}
}
