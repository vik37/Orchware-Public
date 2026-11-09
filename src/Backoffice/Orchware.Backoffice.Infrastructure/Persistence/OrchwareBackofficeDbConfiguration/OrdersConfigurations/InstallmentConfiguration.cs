using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Orders;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.OrdersConfigurations;

public class InstallmentConfiguration : IEntityTypeConfiguration<Installment>
{
	public void Configure(EntityTypeBuilder<Installment> builder)
	{
		builder.ToTable(nameof(Installment),DomainSchema.Order);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property<DateTime>("DueDate")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property<decimal>("Amount")
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property<bool>("IsPaid")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property<DateTime?>("PaidOnDate")
			.HasColumnOrder(5);

		builder.Property<string>("InstallmentCode")
			.HasColumnType("nvarchar(50)")
			.HasColumnOrder(6);

		builder.Property(x => x.PaymentInfoId)
			.HasColumnOrder(7)
			.IsRequired();

		builder.Property<string>("InvoiceUrl")
			.HasColumnType("nvarchar(2048)")
			.HasColumnOrder(8)
			.IsRequired(false);
	}
}
