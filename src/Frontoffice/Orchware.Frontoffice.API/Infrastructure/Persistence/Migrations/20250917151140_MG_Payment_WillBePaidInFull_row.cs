using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_Payment_WillBePaidInFull_row : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WillBePaidInFull",
                table: "Payment",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WillBePaidInFull",
                table: "Payment");
        }
    }
}
