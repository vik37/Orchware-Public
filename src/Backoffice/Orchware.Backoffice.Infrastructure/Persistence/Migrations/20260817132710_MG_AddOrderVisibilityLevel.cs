using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_AddOrderVisibilityLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VisibilityLevel",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Manager");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Status",
                schema: "ordering",
                table: "Order",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Order_VisibilityLevel",
                schema: "ordering",
                table: "Order",
                column: "VisibilityLevel");

			migrationBuilder.Sql(@"
                UPDATE [ordering].[Order]
                SET VisibilityLevel = 'Warehouseman'
                WHERE Status IN ('InPreparation', 'AwaitingDispatch', 'Approved');
            ");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Order_Status",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_VisibilityLevel",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "VisibilityLevel",
                schema: "ordering",
                table: "Order");
        }
    }
}
