using Microsoft.EntityFrameworkCore;
using Orchware.Notification.API.Infrastructure.Persistence.Domain;

namespace Orchware.Notification.API.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
	public DbSet<NotificationEntity> Notifications { get; set; }

	public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
	: base(options)
	{ }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<NotificationEntity>(entity =>
		{
			entity.ToTable("Notifications");

			entity.HasKey(e => e.Id);

			entity.Property(e => e.Title)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(e => e.Message)
				.IsRequired()
				.HasMaxLength(1000);

			entity.Property(e => e.ActionUrl)
				.HasMaxLength(500);

			entity.Property(e => e.TargetRole)
				.HasMaxLength(50);

			entity.Property(e => e.TargetUserId)
				.HasMaxLength(100);

			entity.Property(e => e.TargetGroup)
				.HasMaxLength(100);

			entity.Property(e => e.IsRead)
				.HasDefaultValue(false);

			entity.Property(e => e.Icon)
						.HasMaxLength(50)
						.IsRequired(false);

			entity.Property(x => x.CreatedAt)
							.HasConversion(
								v => v,
								v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
							.IsRequired();

			entity.Property(e => e.ExternalMeesageId)
						 .HasColumnType("uniqueidentifier")
						 .IsRequired();

			entity.HasIndex(e => e.TargetRole)
				.HasDatabaseName("IX_Notification_TargetRole");

			entity.HasIndex(e => e.TargetUserId)
				.HasDatabaseName("IX_Notification_TargetUserId");

			entity.HasIndex(e => e.CreatedAt)
				.HasDatabaseName("IX_Notification_CreatedAt");

			entity.HasIndex(e => e.ExternalMeesageId)
				.IsUnique();
		});

	}
}
