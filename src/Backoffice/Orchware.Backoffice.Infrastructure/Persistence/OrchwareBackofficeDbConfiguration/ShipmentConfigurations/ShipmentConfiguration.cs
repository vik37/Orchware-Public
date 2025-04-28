using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Entities.Shipping;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.ShipmentConfigurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
	public void Configure(EntityTypeBuilder<Shipment> builder)
	{
		builder.ToTable(nameof(Shipment), OrchwareBackofficeDbContext.ShippingSchema);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
		   .HasColumnType("uniqueidentifier")
		   .HasDefaultValueSql("NEWSEQUENTIALID()")
		   .HasColumnOrder(1);

		builder.Property(x => x.OrderId)
		   .HasColumnType("uniqueidentifier")
		   .HasColumnOrder(2);

		builder.Property(x => x.DeliveryStatus)
			.HasColumnType("nvarchar(100)")
			.HasConversion(
				x => x.Value.ToString(),
				x => DeliveryStatus.GetAllStatuses().FirstOrDefault(s => s.Value == x) ?? DeliveryStatus.InTransit
			)
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.CreatedDate)
			.HasColumnOrder(9);

		builder.Property(x => x.ModifiedDate)
			.HasColumnOrder(10);
	}
}
