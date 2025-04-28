using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class CompanyEntityTypeConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasColumnOrder(1)
			.UseIdentityColumn(1,1);

		builder.Property(x => x.UserId)
			.HasColumnOrder(2)
			.HasColumnType("nvarchar(550)")
			.IsRequired();

		builder.HasIndex(x => x.UserId)
				.IsUnique();

		builder.Property(x => x.Name)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.Buyer)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(4)
			.IsRequired();

		builder.Property(x => x.JobTitle)
			.HasColumnType("nvarchar(850)")
			.HasColumnOrder(5)
			.IsRequired();

		builder.Property(x => x.PersonalEmail)
			.HasColumnType("nvarchar(400)")
			.HasColumnOrder(6)
			.IsRequired();

		builder.Property(x => x.CompanyEmail)
			.HasColumnType("nvarchar(400)")
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
