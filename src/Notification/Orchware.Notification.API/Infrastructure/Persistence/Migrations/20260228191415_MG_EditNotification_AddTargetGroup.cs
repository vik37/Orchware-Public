using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Notification.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_EditNotification_AddTargetGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetGroup",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetGroup",
                table: "Notifications");
        }
    }
}
