using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class CompanyEntityTypeConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.ToTable(tb => tb.HasTrigger("[dbo].[TR_Company_UpdateName]"));

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1,1);

		builder.Property(x => x.Name)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.City)
			.HasColumnType("nvarchar(100)")
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.Location)
			.HasColumnType("nvarchar(250)")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.Email)
			.HasColumnType("nvarchar(400)")
			.HasColumnOrder(5);

		builder.Property(x => x.Address)
			.HasColumnType("nvarchar(max)")
			.HasColumnOrder(6);

		builder.Property(x => x.Phone)
			.HasColumnType("nvarchar(30)")
			.HasColumnOrder(7);

		builder.Property(x => x.Budget)
			.HasColumnType("decimal(18,2)")
			.HasColumnOrder(8)
			.IsRequired();

		builder.Property(x => x.CreatedDate)
			.HasColumnOrder(9)
			.IsRequired();

		builder.Property(x => x.ModifiedDate)
			.HasColumnOrder(10);

		builder.HasMany(x => x.Orders)
			.WithOne(x => x.Company)
			.HasForeignKey(x => x.CompanyId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
