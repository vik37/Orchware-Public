using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Notification.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_EditNotificationTable_AddColumns_IconMessageID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExternalMeesageId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExternalMeesageId",
                table: "Notifications",
                column: "ExternalMeesageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_ExternalMeesageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ExternalMeesageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Notifications");
        }
    }
}
