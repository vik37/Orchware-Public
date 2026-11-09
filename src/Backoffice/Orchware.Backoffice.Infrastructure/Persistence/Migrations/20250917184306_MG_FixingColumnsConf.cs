using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_FixingColumnsConf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Products",
                schema: "ordering",
                table: "OrderDetails",
                newName: "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Product",
                schema: "ordering",
                table: "OrderDetails",
                newName: "Products");
        }
    }
}
