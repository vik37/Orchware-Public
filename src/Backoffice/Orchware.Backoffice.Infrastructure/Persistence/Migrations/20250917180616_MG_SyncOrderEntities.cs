using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Backoffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_SyncOrderEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company_Buyer",
                schema: "order",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                schema: "order",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Payment",
                schema: "order",
                table: "Order");

            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                schema: "order",
                newName: "OrderDetails",
                newSchema: "ordering");

            migrationBuilder.RenameTable(
                name: "Order",
                schema: "order",
                newName: "Order",
                newSchema: "ordering");

            migrationBuilder.RenameColumn(
                name: "Company_PersonalEmail",
                schema: "ordering",
                table: "Order",
                newName: "Buyer_PersonalEmail");

            migrationBuilder.RenameColumn(
                name: "Company_JobTitle",
                schema: "ordering",
                table: "Order",
                newName: "Buyer_JobTitle");

            migrationBuilder.RenameColumn(
                name: "Company_CompanyEmail",
                schema: "ordering",
                table: "Order",
                newName: "Buyer_CompanyEmail");

            migrationBuilder.RenameColumn(
                name: "Company_CompanyAddress",
                schema: "ordering",
                table: "Order",
                newName: "Buyer_CompanyAddress");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "ordering",
                table: "Order",
                newName: "PaymentId");

            migrationBuilder.AddColumn<string>(
                name: "ShelfCode",
                schema: "ordering",
                table: "OrderDetails",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AddColumn<int>(
                name: "ShelfProductId",
                schema: "ordering",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "ordering",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)")
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)")
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)")
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "ordering",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                schema: "ordering",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "ordering",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "ordering",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)")
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "ordering",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Buyer_PersonalEmail",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(400)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Buyer_JobTitle",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(850)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Buyer_CompanyEmail",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(400)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                schema: "ordering",
                table: "Order",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AddColumn<string>(
                name: "Buyer_CompanyCity",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Buyer_CompanyLocation",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(250)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Buyer_CompanyName",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(550)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Buyer_CompanyPhoneNumber",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(30)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Buyer_CompanyRegisteredDate",
                schema: "ordering",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Buyer_Name",
                schema: "ordering",
                table: "Order",
                type: "nvarchar(550)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderedDate",
                schema: "ordering",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PaymentInfo",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentNumber = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    WillBePaidInFull = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaidInFull = table.Column<bool>(type: "bit", nullable: false),
                    PaidInFullDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumberOfInstallments = table.Column<int>(type: "int", nullable: true),
                    AmountPerInstallment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalInstallmentDueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Installment",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidOnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstallmentCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    PaymentInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Installment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Installment_PaymentInfo_PaymentInfoId",
                        column: x => x.PaymentInfoId,
                        principalSchema: "ordering",
                        principalTable: "PaymentInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderNumber",
                schema: "ordering",
                table: "Order",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentId",
                schema: "ordering",
                table: "Order",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Installment_PaymentInfoId",
                schema: "ordering",
                table: "Installment",
                column: "PaymentInfoId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentInfo_PaymentId",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropTable(
                name: "Installment",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "PaymentInfo",
                schema: "ordering");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderNumber",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PaymentId",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShelfCode",
                schema: "ordering",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ShelfProductId",
                schema: "ordering",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Buyer_CompanyCity",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Buyer_CompanyLocation",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Buyer_CompanyName",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Buyer_CompanyPhoneNumber",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Buyer_CompanyRegisteredDate",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Buyer_Name",
                schema: "ordering",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderedDate",
                schema: "ordering",
                table: "Order");

            migrationBuilder.EnsureSchema(
                name: "order");

            migrationBuilder.RenameTable(
                name: "OrderDetails",
                schema: "ordering",
                newName: "OrderDetails",
                newSchema: "order");

            migrationBuilder.RenameTable(
                name: "Order",
                schema: "ordering",
                newName: "Order",
                newSchema: "order");

            migrationBuilder.RenameColumn(
                name: "Buyer_PersonalEmail",
                schema: "order",
                table: "Order",
                newName: "Company_PersonalEmail");

            migrationBuilder.RenameColumn(
                name: "Buyer_JobTitle",
                schema: "order",
                table: "Order",
                newName: "Company_JobTitle");

            migrationBuilder.RenameColumn(
                name: "Buyer_CompanyEmail",
                schema: "order",
                table: "Order",
                newName: "Company_CompanyEmail");

            migrationBuilder.RenameColumn(
                name: "Buyer_CompanyAddress",
                schema: "order",
                table: "Order",
                newName: "Company_CompanyAddress");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                schema: "order",
                table: "Order",
                newName: "CompanyId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                schema: "order",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)")
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "order",
                table: "Order",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)")
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                schema: "order",
                table: "Order",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)")
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                schema: "order",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                schema: "order",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "order",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "order",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "order",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<string>(
                name: "Company_PersonalEmail",
                schema: "order",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)");

            migrationBuilder.AlterColumn<string>(
                name: "Company_JobTitle",
                schema: "order",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(850)");

            migrationBuilder.AlterColumn<string>(
                name: "Company_CompanyEmail",
                schema: "order",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                schema: "order",
                table: "Order",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AddColumn<string>(
                name: "Company_Buyer",
                schema: "order",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                schema: "order",
                table: "Order",
                type: "datetime2",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AddColumn<string>(
                name: "Payment",
                schema: "order",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 11);
        }
    }
}
