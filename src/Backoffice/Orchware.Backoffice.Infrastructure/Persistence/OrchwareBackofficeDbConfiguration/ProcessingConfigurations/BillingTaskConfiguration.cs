using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Constants;
using Orchware.Backoffice.Domain.Entities.Proccessing;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.ProcessingConfigurations;

public class BillingTaskConfiguration : IEntityTypeConfiguration<BillingTask>
{
	public void Configure(EntityTypeBuilder<BillingTask> builder)
	{
		builder.ToTable(nameof(BillingTask), DomainSchema.Processing);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.OrderId)
		   .HasColumnType("uniqueidentifier")
		   .HasColumnOrder(2)
		   .IsRequired();

		builder.Property(x => x.Status)
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.IsFullPayment)
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.InstallmentId)
			.HasColumnOrder(5)
			.IsRequired(false);

		builder.Property(x => x.InstalmentNumber)
			.HasColumnOrder(6)
			.IsRequired(false);

		builder.Property(x => x.NumberOfInstallments)
			.HasColumnOrder(7)
			.IsRequired(false);

		builder.Property(x => x.ScheduledFor)
			.HasColumnOrder(8)
			.IsRequired(false);

		builder.Property(x => x.RetryCount)
			.HasColumnOrder(9)
			.HasDefaultValue(0);

		builder.Property(x => x.LastErrorMessage)
			.HasColumnType("NVARCHAR(MAX)")
			.HasColumnOrder(10)
			.IsRequired(false);

		builder.Property(x => x.CreatedDate)
			.HasColumnOrder(11)
			.IsRequired();

		builder.Property(x => x.ProcessedAt)
			.HasColumnOrder(12)
			.IsRequired(false);

		builder.HasIndex(x => new { x.Status, x.InstallmentId });
	}
}
