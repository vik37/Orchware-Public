using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_AlterTablePaymentRule_AddColumnAllowDiscountModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowDiscountModification",
                schema: "settings",
                table: "PaymentRules",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowDiscountModification",
                schema: "settings",
                table: "PaymentRules");
        }
    }
}
