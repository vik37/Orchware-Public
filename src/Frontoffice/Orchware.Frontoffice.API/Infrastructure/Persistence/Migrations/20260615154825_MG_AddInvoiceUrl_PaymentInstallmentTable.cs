using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_AddInvoiceUrl_PaymentInstallmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                table: "Payment",
                type: "nvarchar(2048)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 15);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                table: "Installment",
                type: "nvarchar(2048)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                table: "Installment");
        }
    }
}
