using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Backoffice.Domain.Entities.Inventory;

namespace Orchware.Backoffice.Infrastructure.Persistence.OrchwareBackofficeDbConfiguration.InventoryConfigurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
	public void Configure(EntityTypeBuilder<Shelf> builder)
	{
		builder.ToTable(nameof(Shelf), OrchwareBackofficeDbContext.InventorySchema);

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1, 1);

		builder.Property(x => x.Code)
			.HasColumnType("nvarchar(20)")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.SeasonalFruits)
			.HasConversion<int>()
			.HasColumnOrder(3)
			.IsRequired();

		builder.HasMany(x => x.Products)
			.WithOne(x => x.Shelf)
			.HasForeignKey(x => x.ShelfId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasIndex(x => x.Code).IsUnique();

		builder.HasIndex(x => x.SeasonalFruits)
			.HasDatabaseName("IX_SeasonalFruits")
			.IsUnique(false);
	}
}
