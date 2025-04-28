using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.Name)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.SeasonalFruits)
			.HasConversion<int>()
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.AvailableQuantity)
			.HasColumnType("real")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.Price)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.MinQuantity)
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.Units)
			.HasConversion<int>()
			.HasColumnOrder(7)
			.IsRequired();

		builder.Property(x => x.Image)
			.HasColumnType("nvarchar(max)")
			.HasColumnOrder(8)
			.IsRequired();

		builder.HasIndex(x => x.SeasonalFruits)
			.HasDatabaseName("IX_SeasonalFruits")
			.IsUnique(false);
	}
}
