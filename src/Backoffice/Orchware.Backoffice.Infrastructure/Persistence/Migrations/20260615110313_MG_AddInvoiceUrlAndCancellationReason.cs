using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_AddInvoiceUrlAndCancellationReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                schema: "ordering",
                table: "PaymentInfo",
                type: "nvarchar(2048)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                schema: "ordering",
                table: "Installment",
                type: "nvarchar(2048)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                schema: "ordering",
                table: "PaymentInfo");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                schema: "ordering",
                table: "Installment");
        }
    }
}
