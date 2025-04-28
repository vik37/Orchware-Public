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

		builder.Property(x => x.CustomerId)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.OrderId)
		   .HasColumnType("uniqueidentifier")
		   .HasColumnOrder(3)
		   .IsRequired();

		builder.Property(x => x.PaymentDate)
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.PayImmediately)
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.SelectedPaymentDate)
			.HasColumnOrder(6);

		builder.Property(x => x.MaxsimumAllowedNumberOfMonths)
			.HasColumnOrder(7)
			.IsRequired();

		builder.HasIndex(x => new {x.OrderId, x.CustomerId}).IsUnique();
	}
}
