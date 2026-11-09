using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Settings;
using Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.Shared;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.SettingsCongiguration;

public class PaymentRulesyConfiguration : IEntityTypeConfiguration<PaymentRules>
{
	public void Configure(EntityTypeBuilder<PaymentRules> builder)
	{
		builder.ToTable(nameof(PaymentRules), DomainSchema.Settings);

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

		builder.Property(x => x.MinimumOrderAmountForDiscount)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(4);

		builder.Property(x => x.OrderAmountDiscountPercentage)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(5);

		builder.Property(x => x.PayImmediatelyDiscountPercentage)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(6);

		builder.Property(x => x.AllowDiscountModification)
			.HasColumnOrder(7)
			.HasDefaultValue(false)
			.IsRequired();
	}
}
