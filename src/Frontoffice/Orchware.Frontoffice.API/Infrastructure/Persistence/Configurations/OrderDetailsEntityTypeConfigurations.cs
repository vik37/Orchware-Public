using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations
{
	public class OrderDetailsEntityTypeConfigurations : IEntityTypeConfiguration<OrderDetails>
	{
		public void Configure(EntityTypeBuilder<OrderDetails> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Id)
				.HasColumnOrder(1)
				.UseIdentityColumn(1, 1);

			builder.Property(x => x.OrderId)
			   .HasColumnType("uniqueidentifier") 
			   .HasColumnOrder(2)
			   .IsRequired();

			builder.Property(x => x.ProductId)
				.HasColumnOrder(3)
				.IsRequired();

			builder.Property(x => x.Quantity)
				.HasColumnType("real")
				.HasColumnOrder(4)
				.IsRequired();

			builder.Property(x => x.PricePerUnit)
				.HasColumnType("decimal(18,2)")
				.HasColumnOrder(5)
				.IsRequired();

			builder.Property(x => x.UnitOfMeasure)
				.HasColumnType("nvarchar(50)")
				.HasColumnOrder(6)
				.IsRequired();

			builder.Property(x => x.TotalPrice)
				.HasColumnType("decimal(18,2)")
				.HasColumnOrder(7)
				.IsRequired();

			builder.HasOne(x => x.Product)
				.WithMany()
				.HasForeignKey(x => x.ProductId);
		}
	}
}
