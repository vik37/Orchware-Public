using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_FixingOrderPayment_OrderIdInPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentInfo_PaymentId",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentId",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                schema: "ordering",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInfo_OrderId",
                schema: "ordering",
                table: "PaymentInfo",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentInfo_Order_OrderId",
                schema: "ordering",
                table: "PaymentInfo",
                column: "OrderId",
                principalSchema: "ordering",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentInfo_Order_OrderId",
                schema: "ordering",
                table: "PaymentInfo");

            migrationBuilder.DropIndex(
                name: "IX_PaymentInfo_OrderId",
                schema: "ordering",
                table: "PaymentInfo");

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                schema: "ordering",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentId",
                schema: "ordering",
                table: "Order",
                column: "PaymentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentInfo_PaymentId",
                schema: "ordering",
                table: "Order",
                column: "PaymentId",
                principalSchema: "ordering",
                principalTable: "PaymentInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
