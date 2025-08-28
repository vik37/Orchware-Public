using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orchware.Frontoffice.API.Domain;

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.Property(x => x.Id)
		   .HasColumnType("uniqueidentifier")
		   .ValueGeneratedNever()
		   .HasColumnOrder(1);

		builder.Property(x => x.Name)
			.HasColumnType("nvarchar(550)")
			.HasColumnOrder(2)
			.IsRequired();

		builder.Property(x => x.JobTitle)
			.HasColumnType("nvarchar(850)")
			.HasColumnOrder(3)
			.IsRequired();

		builder.Property(x => x.PersonalEmail)
			.HasColumnType("nvarchar(400)")
			.HasColumnOrder(4)
			.IsRequired();

		builder.HasOne(x => x.Company)
			.WithMany(x => x.Users)
			.HasForeignKey(x => x.CompanyId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
