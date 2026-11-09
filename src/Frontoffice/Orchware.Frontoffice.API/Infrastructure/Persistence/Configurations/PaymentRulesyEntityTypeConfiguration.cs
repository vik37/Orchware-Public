using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;
using Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations.Shared;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class PaymentRulesyEntityTypeConfiguration : IEntityTypeConfiguration<PaymentRules>
{
	public void Configure(EntityTypeBuilder<PaymentRules> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.MaxAllowedInstallments)
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.GracePeriod)
			.HasConversion(new TimeSpanToDaysConverter())
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.MaximumPaymentPeriod)
			.HasConversion(new TimeSpanToDaysConverter())
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.MinimumOrderAmountForDiscount)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(5);

		builder.Property(x => x.OrderAmountDiscountPercentage)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(6);

		builder.Property(x => x.PayImmediatelyDiscountPercentage)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(7);
	}
}
