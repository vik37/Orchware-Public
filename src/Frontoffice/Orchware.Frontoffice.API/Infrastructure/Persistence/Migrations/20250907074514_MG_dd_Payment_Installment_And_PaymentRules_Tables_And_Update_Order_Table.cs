using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchware.Frontoffice.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MG_dd_Payment_Installment_And_PaymentRules_Tables_And_Update_Order_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "SelectedPaymentDate",
                table: "Payment",
                newName: "PaidInFullDate");

            migrationBuilder.RenameColumn(
                name: "PayImmediately",
                table: "Payment",
                newName: "IsPaidInFull");

            migrationBuilder.RenameColumn(
                name: "MaxsimumAllowedNumberOfMonths",
                table: "Payment",
                newName: "PaymentRuleId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Payment",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_OrderId_CustomerId",
                table: "Payment",
                newName: "IX_Payment_OrderId_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidInFullDate",
                table: "Payment",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaidInFull",
                table: "Payment",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentRuleId",
                table: "Payment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPerInstallment",
                table: "Payment",
                type: "decimal(18,2)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 12);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalInstallmentDueDate",
                table: "Payment",
                type: "datetime2",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Payment",
                type: "datetime2",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfInstallments",
                table: "Payment",
                type: "int",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AddColumn<string>(
                name: "PaymentNumber",
                table: "Payment",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Order",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Quantity] * [PricePerUnit]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "Installment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    InstallmentCode = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    PaidOnDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Installment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Installment_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaxAllowedInstallments = table.Column<int>(type: "int", nullable: false),
                    GracePeriod = table.Column<int>(type: "int", nullable: false),
                    MaximumPaymentPeriod = table.Column<int>(type: "int", nullable: false),
                    MinimumOrderAmountForDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderAmountDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayImmediatelyDiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentRuleId",
                table: "Payment",
                column: "PaymentRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Installment_PaymentId",
                table: "Installment",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentRules_PaymentRuleId",
                table: "Payment",
                column: "PaymentRuleId",
                principalTable: "PaymentRules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentRules_PaymentRuleId",
                table: "Payment");

            migrationBuilder.DropTable(
                name: "Installment");

            migrationBuilder.DropTable(
                name: "PaymentRules");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentRuleId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "AmountPerInstallment",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "FinalInstallmentDueDate",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "NumberOfInstallments",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentNumber",
                table: "Payment");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Payment",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "PaymentRuleId",
                table: "Payment",
                newName: "MaxsimumAllowedNumberOfMonths");

            migrationBuilder.RenameColumn(
                name: "PaidInFullDate",
                table: "Payment",
                newName: "SelectedPaymentDate");

            migrationBuilder.RenameColumn(
                name: "IsPaidInFull",
                table: "Payment",
                newName: "PayImmediately");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_OrderId_UserId",
                table: "Payment",
                newName: "IX_Payment_OrderId_CustomerId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<int>(
                name: "MaxsimumAllowedNumberOfMonths",
                table: "Payment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SelectedPaymentDate",
                table: "Payment",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<bool>(
                name: "PayImmediately",
                table: "Payment",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComputedColumnSql: "[Quantity] * [PricePerUnit]");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNumber",
                table: "Order",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)")
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<int>(
                name: "Discount",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m)
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m)
                .Annotation("Relational:ColumnOrder", 6);
        }
    }
}
